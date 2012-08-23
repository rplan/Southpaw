var Southpaw = Southpaw || Southpaw || {};
Southpaw.Runtime = Southpaw.Runtime || Southpaw.Runtime || {};
Southpaw.Runtime.Clientside = Southpaw.Runtime.Clientside || Southpaw.Runtime.Clientside || {};

Southpaw.Runtime.Clientside.EventUtils = function () {
    this._callbacks = {};

    this.bind = function (eventName, callback) {
        if (this._callbacks[eventName] === undefined) {
            this._callbacks[eventName] = [];
        }
        if (!this._callbacks[eventName].contains(callback)) {
            this._callbacks[eventName].add(callback);
        }
    };

    this.clear = function () {
        Object.clearKeys(this._callbacks);
    };

    this.unbind = function (eventName, callback) {
        if (this._callbacks[eventName] === undefined) {
            return;
        }
        var idx = -1;
        for (var i = 0; i < this._callbacks[eventName].length; i++) {
            if (this._callbacks[eventName][i] === callback) {
                idx = i;
                break;
            }
        }
        if (idx > -1) {
            this._callbacks[eventName].removeAt(idx);
        }
    };

    this.trigger = function (eventName, evt) {
        if (this._callbacks[eventName] === undefined) {
            return;
        }
        for(var callback in this._callbacks[eventName]) {
            callback(evt);
        }
    };
};

Southpaw.Runtime.Clientside.ViewModel = function () {
    this._eventUtils = new Southpaw.Runtime.Clientside.EventUtils();
    this.attrs = {};
    var _previousAttrs = {};
};

Southpaw.Runtime.Clientside.ViewModel.prototype = {
    get: function (propertyName) {
        return this.attrs[propertyName];
    },
    set: function (attributes, options) {
        options = options || {};
        if (!attributes) return true;
        var currentAttrs = this.attrs;

        // validate if requested
        if (!options.isSilent && options.validate && this.validate(attributes, options))
            return false;

        for (var attributeName in attributes) {
            var newValue = attributes[attributeName];
            if (!_.isEqual(currentAttrs[attributeName], newValue)) {
                currentAttrs[attributeName] = newValue;
                this.hasChanged = true;
                if (!options.isSilent) {
                    // TODO: arguments should be a jquery event
                    this.trigger('change:' + attributeName, this, newValue, options);
                }
            }
        }
        if (!options.isSilent) {
            this.change(options);
        }
        return true;
    },
    setProperty: function (propertyName, value) {
        var p = {};
        p[propertyName] = value;
        this.set(p);
    },

    getProperty: function (propertyName) {
        return this.attrs[propertyName];
    },

    validate: function (attributes, options) {
        return true;
    },

    change: function (options) {
        // TODO: arguments should be a jquery event
        this.trigger('change', this, options);
        _previousAttrs = _.clone(this.attrs);
        this.hasChanged = false;
    },

    hasPropertyChanged: function (propertyName) {
        return _previousAttrs[propertyName] != this.attrs[propertyName];
    },

    setPropertyFromString: function (propertyName, newValue, type) {
        var n;
        var p = {};
        switch (type.name.toLowerCase()) {
            case 'string':
                p[propertyName] = newValue;
                this.set(p);
                return;
            case 'int':
                n = parseInt(newValue);
                if (!isNaN(n)) {
                    p[propertyName] = n;
                    this.set(p);
                }
                return;
            case 'float':
                n = parseFloat(newValue);
                if (!isNaN(n)) {
                    p[propertyName] = n;
                    this.set(p);
                }
                return;
            case 'date':
                n = newValue.split('/');
                if (n.length == 3
                    && (n[0].length == 1 || n[0].length == 2)
                    && (n[1].length == 1 || n[0].length == 2)
                    && (n[2].length == 4)) {
                    var d = parseInt(n[0]),
                        m = parseInt(n[1]),
                        y = parseInt(n[2]);
                    n = new Date(y, m, d);
                    p[propertyName] = n;
                    this.set(p);
                }

            default:
                throw "Unknown type '" + type + "' used to set property '" + propertyName + "'";
        }
    },

    clear: function (options) {
        options = options || {};
        var old = this.attrs;
        var validObj = {};
        for (var attr in old) validObj[attr] = void 0;

        // validate if requested
        if (!options.isSilent && options.validate && this.validate(validObj, options))
            return false;

        this.attrs = {};

        if (!options.isSilent) {
            for (var attributeName in old) {
                // TODO: arguments should be a jquery event
                this.trigger('change:' + attributeName, this, void 0, options);
            }
            this.change(options);
        }
    },

    bind: function (eventName, callback) {
        this._eventUtils.bind(eventName, callback);
    },

    clearEvents: function () {
        this._eventUtils.clear();
    },

    unbind: function (eventName, callback) {
        this._eventUtils.unbind(eventName, callback);
    },

    trigger: function (eventName, evt) {
        this._eventUtils.trigger(eventName, evt);
    },

    toJSON: function () {
        var json = _.clone(this.attrs);
        for(x in json) {
            if (!json.hasOwnProperty(x))
                continue;
            if (json[x] && json[x].toJSON)
                json[x] = json[x].toJSON();
        }
        return json;
    }
};

Southpaw.Runtime.Clientside.Service = function () {
    this._onErrorCallbacks = [];
    this._onSuccessCallbacks = [];

    this.addOnErrorCallback = function (callback) {
        if (callback)
            this._onErrorCallbacks.push(callback);
    };
    this.addOnSuccessCallback = function (callback) {
        if (callback)
            this._onSuccessCallbacks.push(callback);
    };
    //this.instantiateModel = function (returnValue) {
    //return returnValue;
    //};

    this.doCall = function (data) {
        // generator behaviour is to only generate an override if the method is POST
        var type = this.get_httpMethod ? this.get_httpMethod() : "GET";
        // Default JSON-request options.
        var params = {
            dataType: 'json',
            url: this.getUrl(),
            processData: false,
            type: type
        };

        if (type.toLowerCase() == 'post') {
            if (data.toJSON !== undefined) {
                data = JSON.stringify(data.toJSON());
            } else {
                data = JSON.stringify(data);
            }
            params.data = data;
            params.contentType = 'application/json';
        } else {
            if (data) {
                if (data.toJSON !== undefined) {
                    data = JSON.stringify(data.toJSON());
                } else {
                    data = JSON.stringify(data);
                }
                params.url += "/" + data;
            }
        }

        var that = this;
        if (this._onErrorCallbacks.length > 0) {
            params.error = function (jqXhr, textStatus, errorThrown) {
                for (var i = 0; i < that._onErrorCallbacks.length; i++) {
                    that._onErrorCallbacks[i].call(that);
                }
            };
        }

        if (this._onSuccessCallbacks.length > 0) {
            params.success = function (resp, textStatus) {
                // assume response is JSON
                for (var i = 0; i < that._onSuccessCallbacks.length; i++) {
                    that._onSuccessCallbacks[i].call(that, resp);
                }
            };
        }


        // Make the request.
        return $.ajax(params);
    };
};

Southpaw.Runtime.Clientside.ViewModelCollection = function () {
    this._eventUtils = new Southpaw.Runtime.Clientside.EventUtils();
    this.items = [];
    var _previousItems = [];
};

// TODO: no concept of identity; so hard/impossible to determine whether duplicates exist in a list (should throw an error if duplicates exist)
Southpaw.Runtime.Clientside.ViewModelCollection.prototype = {
    add: function (item) {
        this.items.push(item);
        // TODO argument to trigger should be jquery event
        this.trigger('add', item);
    },
    addRange: function (items) {
        for (var i = 0; i < items.length; i++) {
            if (items[i])
                this.add(items[i]);
        }
    },
    remove: function (item) {
        var idx = this._indexOf(item);
        if (idx >= 0) {
            var removedItems = this.items.splice(idx, 1);
            // TODO: should be a jquery event
            this.trigger('remove', removedItems);
            return 1;
        }
        return 0;
    },

    removeAt: function (idx) {
        var removedItems = this.items.splice(idx, 1);
        // TODO: should be a jquery event
        this.trigger('remove', removedItems);
        return removedItems.length;
    },
    clear: function () {
        var old = this.clone();
        this.items = [];
        // TODO: should be a jquery event
        this.trigger('remove', this.clone());
        this.trigger('clear');
    },
    contains: function (item) {
        return this._indexOf(item) >= 0;
    },
    clone: function () {
        return this.items.slice(0, this.items.length);
    },
    get_count: function (){
        return this.items.length;
    },

    _indexOf: function (item) {
        if (this.items.indexOf)
            return this.items.indexOf(item);
        for (var i = 0; i < this.items.length; i++) {
            if (this.items[i] == item)
                return i;
        }
        return -1;
    },

    bind: function (eventName, callback) {
        this._eventUtils.bind(eventName, callback);
    },

    clearEvents: function () {
        this._eventUtils.clear();
    },

    unbind: function (eventName, callback) {
        this._eventUtils.unbind(eventName, callback);
    },

    trigger: function (eventName, evt) {
        this._eventUtils.trigger(eventName, evt);
    }
};
