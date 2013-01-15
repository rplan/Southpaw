var Southpaw = Southpaw || Southpaw || {};
Southpaw.Runtime = Southpaw.Runtime || Southpaw.Runtime || {};
Southpaw.Runtime.Clientside = Southpaw.Runtime.Clientside || Southpaw.Runtime.Clientside || {};

Southpaw.Runtime.Clientside.GlobalSettings = function() {

};
Southpaw.Runtime.Clientside.GlobalSettings.serviceSettings = {};
Southpaw.Runtime.Clientside.GlobalSettings.viewSettings = {};
Southpaw.Runtime.Clientside.GlobalSettings.viewModelSettings = {};
Southpaw.Runtime.Clientside.EventUtils = function () {
    this._callbacks = {};

    this.bind = function (eventName, callback, ctx) {
        if (this._callbacks[eventName] === undefined) {
            this._callbacks[eventName] = [];
        }
        if (!this._callbacks[eventName].contains(callback)) {
            this._callbacks[eventName].add([callback, ctx]);
        }
    };

    this.clear = function () {
        Object.clearKeys(this._callbacks);
    };

    this.unbind = function (eventName, callback, ctx) {
        if (this._callbacks[eventName] === undefined) {
            return;
        }
        var idx = -1;
        for (var i = 0; i < this._callbacks[eventName].length; i++) {
            if (this._callbacks[eventName][i][0] === callback && this._callbacks[eventName][i][1] == ctx) {
                idx = i;
                break;
            }
        }
        if (idx > -1) {
            this._callbacks[eventName].removeAt(idx);
        }
    };

    this.trigger = function (eventName, evt) {
        if (this._callbacks[eventName] === undefined && this._callbacks['all'] === undefined) {
            return;
        }
        var len;
        if (eventName !== 'all' && this._callbacks[eventName] !== undefined) {
            len = this._callbacks[eventName].length;
            for (var i = 0; i < len; i++) {
                this._callbacks[eventName][i][0].call(this._callbacks[eventName][i][1], evt);
            }
        }
        if (this._callbacks['all'] !== undefined) {
            len = this._callbacks['all'].length;
            for (var j = 0; j < len; j++) {
                // todo: will only return pass first arg to call back. should pass all.
                this._callbacks['all'][j][0].call(this._callbacks['all'][j][1], eventName, evt);
            }
        }
    };
};

Southpaw.Runtime.Clientside.CidCounter = 0;

Southpaw.Runtime.Clientside.ViewModel$1 = function () {
    this._eventUtils = new Southpaw.Runtime.Clientside.EventUtils();
    this.attrs = {};
    this._previousAttrs = {};
    this._changed = {};
    this.cid = ++Southpaw.Runtime.Clientside.CidCounter;
    this._collections = [];
    this.errors = new Southpaw.Runtime.Clientside.ValidationResults();
};
Southpaw.Runtime.Clientside.ViewModel$1.__typeName = 'ViewModel';

Southpaw.Runtime.Clientside.ViewModel$1.prototype = {
    get: function (propertyName) {
        return this.attrs[propertyName];
    },
    set: function (attributes, options) {
        options = options || {};
        if (!attributes) return true;
        var currentAttrs = this.attrs;

        // validate if requested
        if (!options.silent && options.validate && this.validate(attributes, options))
            return false;

        for (var attributeName in attributes) {
            var newValue = attributes[attributeName];
            if (!_.isEqual(currentAttrs[attributeName], newValue)) {
                currentAttrs[attributeName] = newValue;
                this._changed[attributeName] = true;
            }
        }
        if (!options.silent) {
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
        if (_.isEmpty(this._changed))
            return;
        // TODO: arguments should be a jquery event
        for (var attr in this._changed) {
            this.trigger('change:' + attr, this, this.get(attr), options);
            delete this._changed[attr];
        }
        this.trigger('change', this, options);
        this._previousAttrs = _.clone(this.attrs);
    },

    hasChanged: function (propertyName) {
        if (propertyName)
            return _.has(this._changed, propertyName);
        return !_.isEmpty(this._changed);
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
                n = Southpaw.Runtime.Clientside.Validation.TypeConverters.toInt(newValue);
                if (n !== null) {
                    p[propertyName] = n;
                    this.set(p);
                }
                return;
            case 'float':
                n = Southpaw.Runtime.Clientside.Validation.TypeConverters.toFloat(newValue);
                if (n !== null) {
                    p[propertyName] = n;
                    this.set(p);
                }
                return;
            case 'date':
                n = Southpaw.Runtime.Clientside.Validation.TypeConverters.toDate(newValue);
                if (n !== null) {
                    p[propertyName] = n;
                    this.set(p);
                }
            case 'bool':
                n = Southpaw.Runtime.Clientside.Validation.TypeConverters.toBool(newValue);
                if (n !== null) {
                    p[propertyName] = n;
                    this.set(p);
                }
            case 'char':
                n = Southpaw.Runtime.Clientside.Validation.TypeConverters.toChar(newValue);
                if (n !== null) {
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
        if (!options.silent && options.validate && this.validate(validObj, options))
            return false;

        this.attrs = {};

        if (!options.silent) {
            for (var attributeName in old) {
                // TODO: arguments should be a jquery event
                this.trigger('change:' + attributeName, this, void 0, options);
            }
            this.change(options);
        }
    },

    bind: function (eventName, callback, ctx) {
        this._eventUtils.bind(eventName, callback, ctx || this);
    },

    clearEvents: function () {
        this._eventUtils.clear();
    },

    unbind: function (eventName, callback, ctx) {
        this._eventUtils.unbind(eventName, callback, ctx || this);
    },

    trigger: function (eventName, evt) {
        this._eventUtils.trigger(eventName, evt);
    },

    toJSON: function () {
        var json = _.clone(this.attrs);
        for(var x in json) {
            if (!json.hasOwnProperty(x))
                continue;
            if (json[x] && json[x].toJSON)
                json[x] = json[x].toJSON();
            if (json[x] instanceof Array) {
                var y = [];
                for(var i = 0; i < json[x].length; i++) {
                    if (json[x][i] && json[x][i].toJSON)
                        y.push(json[x][i].toJSON());
                    else
                        y.push(json[x][i]);
                }
                json[x] = y;
            }
        }
        json['Cid'] = this.cid;
        return json;
    },
    setCollection: function (collection) {
        if (this._collections.indexOf(collection)<0)
            this._collections.push(collection);
    },
    unsetCollection: function (collection) {
        var idx = this._collections.indexOf(collection) > 0;
        if (idx > -1) {
            this._collections.splice(idx, 1);
        }
    }
};

Southpaw.Runtime.Clientside.Service = function () {
    this._onErrorCallbacks = [];
    this._onSuccessCallbacks = [];
    this._options = {};

    this.addOnErrorCallback = function (callback) {
        if (callback)
            this._onErrorCallbacks.push(callback);
    };
    this.addOnSuccessCallback = function (callback) {
        if (callback)
            this._onSuccessCallbacks.push(callback);
    };
    this.getJsonReviver = function () {
        return this._reviver;
    };
    this.initialise = function(serviceOptions) {
        if (serviceOptions) {
            this._options = _.extend({ }, this._options, serviceOptions);
        }
    };
    //this.instantiateModel = function (returnValue) {
    //return returnValue;
    //};

    this.doCall = function (data) {
        var url = this._options.url || this.getUrl();
        var jsonReviver = this._options.jsonReviver || this.getJsonReviver() || Southpaw.Runtime.Clientside.GlobalSettings.serviceSettings[Southpaw.Runtime.Clientside.ServiceSettingName.jsonReviver];
        var httpMethod = this._options.httpMethod || this.get_httpMethod() || Southpaw.Runtime.Clientside.GlobalSettings.serviceSettings[Southpaw.Runtime.Clientside.ServiceSettingName.httpMethod] || 'GET';
        // generator behaviour is to only generate an override if the method is POST
        var type = httpMethod;
        // Default JSON-request options.
        var params = {
            //dataType: 'json',
            dataType: 'text',
            url: url,
            processData: false,
            type: type
        };

        if (type.toLowerCase() == 'post') {
            data = JSON.stringify(data);
            params.data = data;
            params.contentType = 'application/json';
        } else {
            if (data) {
                data = JSON.stringify(data);
                params.url += "/" + data;
            }
        }

        var that = this;
        if (this._onErrorCallbacks.length > 0) {
            params.error = function (jqXhr, textStatus, errorThrown) {
                var data;
                if (jqXhr.responseText) {
                    try {
                        if (jsonReviver)
                            data = JSON.parse(resp, jsonReviver);
                        else
                            data = JSON.parse(resp);
                    } catch(e) {
                        
                    }
                }
                for (var i = 0; i < that._onErrorCallbacks.length; i++) {
                    that._onErrorCallbacks[i].call(that, data, jqXhr);
                }
            };
        }

        if (this._onSuccessCallbacks.length > 0) {
            params.success = function (resp, textStatus) {
                if (jsonReviver)
                    data = JSON.parse(resp, jsonReviver);
                else
                    data = JSON.parse(resp);
                // assume response is JSON
                for (var i = 0; i < that._onSuccessCallbacks.length; i++) {
                    that._onSuccessCallbacks[i].call(that, data);
                }
            };
        }

        // Make the request.
        return $.ajax(params);
    };
};

Southpaw.Runtime.Clientside.ViewModelCollection = function (type) {
    this._eventUtils = new Southpaw.Runtime.Clientside.EventUtils();
    this.items = [];
    var _previousItems = [];
    this.type = type;
    this._index = -1;
};

// TODO: no concept of identity; so hard/impossible to determine whether duplicates exist in a list (should throw an error if duplicates exist)
Southpaw.Runtime.Clientside.ViewModelCollection.prototype = {
    add: function (item, options) {
        this.items.push(item);
        item.setCollection(this);
        item.bind('all', this._modelEvent, this);
        // TODO argument to trigger should be jquery event
        if (!options || (options && !options.silent)) {
            this.trigger('add', item);
            this.trigger('change');
        }
    },
    addRange: function (items, options) {
        for (var i = 0; i < items.length; i++) {
            if (items[i]) {
                this.add(items[i], _.extend({}, options, { silent: true }));
                if (!options || (options && !options.silent)) {
                    var all = this._eventUtils._callbacks['all'];
                    this._eventUtils._callbacks['all'] = undefined;
                    this.trigger('add', items[i]);
                    this.trigger('change');
                    this._eventUtils._callbacks['all'] = all;
                    this.trigger('all');
                }
            }
        }
    },
    remove: function (item, options) {
        var idx = this._indexOf(item);
        if (idx >= 0) {
            var removedItem = this.items.splice(idx, 1)[0];
            removedItem.unsetCollection(this);
            item.unbind('all', this._modelEvent, this);
            // TODO: should be a jquery event
            if (!options || (options && !options.silent)) {
                this.trigger('remove', removedItem);
                this.trigger('change');
            }
            return 1;
        }
        return 0;
    },
    removeAt: function (idx, options) {
        var removedItems = this.items.splice(idx, 1);
        // TODO: should be a jquery event
        if (!options || (options && !options.silent)) {
            this.trigger('remove', removedItems);
            this.trigger('change');
        }
        return removedItems.length;
    },
    elementAt: function (idx) {
        return this.items[idx];
    },
    clear: function (options) {
        var old = this.clone();
        this.items = [];
        // TODO: should be a jquery event
        if (!options || (options && !options.silent)) {
            this.trigger('remove', this.clone());
            this.trigger('clear');
            this.trigger('change');
        }
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
    _modelEvent: function (eventName, evt) {
        this.trigger.apply(this, arguments);
    },

    bind: function (eventName, callback, ctx) {
        this._eventUtils.bind(eventName, callback, ctx || this);
    },

    clearEvents: function () {
        this._eventUtils.clear();
    },

    unbind: function (eventName, callback, ctx) {
        this._eventUtils.unbind(eventName, callback, ctx || this);
    },

    trigger: function (eventName, evt) {
        this._eventUtils.trigger(eventName, evt);
    },
    set: function (jsonArray, options) {
        // TODO: should handle updates to collection children,
        // based on PK and/or CID
        if (jsonArray.length === undefined)
            throw "Attempting to pass an argument into ViewModelCollection.set which isn't an array of JSON objects"
        this.clear({ silent: true });
        for (var i = 0; i < jsonArray.length; i++) {
            var m = new this.type();
            // TODO: merge silent: true and options argument above
            m.set(jsonArray[i], { silent: true });
            this.add(m, { silent: true });
        }
        if (!options || (options && !options.silent)) {
            this.trigger('change');
        }
    },
    setFromJSON: function (jsonArray, options) {
        // TODO: should handle updates to collection children,
        // based on PK and/or CID
        if (jsonArray.length === undefined)
            throw "Attempting to pass an argument into ViewModelCollection.setFromJSON which isn't an array of JSON objects"
        this.clear({ silent: true });
        for (var i = 0; i < jsonArray.length; i++) {
            var m = new this.type();
            // TODO: merge silent: true and options argument above
            m.SetFromJSON(jsonArray[i], { silent: true });
            this.add(m, { silent: true });
        }
        if (!options || (options && !options.silent)) {
            this.trigger('change');
        }
    },
    toJSON: function () {
        var arr = [];
        for(var i = 0; i < this.items.length; i++) {
            arr.push(this.items[i].toJSON());
        }
        return arr;
    },
    getById: function (id) {
        for(var i = 0; i < this.items.length; i++) {
            if (this.items[i].get("Id") == id)
                return this.items[i];
        }
        return null;
    },
    getByCid: function (cid) {
        for(var i = 0; i < this.items.length; i++) {
            if (this.items[i].cid == cid)
                return this.items[i];
        }
        return null;
    },
    getEnumerator: function () {
        var that = this;
        var _index = -1;
        return {
            moveNext: function () {
                _index++;
                return (_index < that.items.length);
            },
            reset: function () {
                _index = -1;
            },
            get_current: function () {
                if (_index < 0 || _index >= that.items.length)
        			throw 'Invalid operation';
        		var k = that.items[_index];
        	    return k;
        	},
            dispose: function () {
            }
        };
    }
};

Type.registerClass(window, 'Southpaw.Runtime.Clientside.ViewModelCollection', Southpaw.Runtime.Clientside.ViewModelCollection, null, [ss_IEnumerable]);

Southpaw.Runtime.Clientside.ViewOptions = function () {
};

Southpaw.Runtime.Clientside.ViewOptions.prototype = {
	set_elementSelector: function(value) {
		this.element = ($(value))[0];
	},
	toJSON: function () {
	    var json = {};
	    for (var prop in this) {
	        if (this[prop].toJSON)
	            json[prop] = this[prop].toJSON();
	    }
	    return json;
	}
};

Southpaw.Runtime.Clientside.ValidationResults = function() {
    this.isError = false;
    this.errorMessages = [];
    this.errorsByProperty = {};
};

Southpaw.Runtime.Clientside.ValidationResults.prototype = {
    addError: function(propertyName, errorMessage) {
        this.errorMessages.push(errorMessage);
        this.errorsByProperty[propertyName] = this.errorsByProperty[propertyName] || [];
        this.errorsByProperty[propertyName].push(errorMessage);
        this.isError = true;
        return this;
    },
    clear: function() {
        this.isError = false;
        this.errorMessages = [];
        this.errorsByProperty = {};
        return this;
    }
};

Southpaw.Runtime.Clientside.ViewOptions$1 = function(TModel) {
	//var $type = function() {
		//this.model = TModel.getDefaultValue();
		//Southpaw.Runtime.Clientside.ViewOptions.call(this);
	//};
	//$type.registerGenericClassInstance($type, Southpaw.Runtime.Clientside.ViewOptions$1, [TModel], function() {
		//return Southpaw.Runtime.Clientside.ViewOptions;
	//}, function() {
		//return [];
	//});
	//return $type;
};
//Southpaw.Runtime.Clientside.ViewOptions$1.registerGenericClass('Southpaw.Runtime.Clientside.ViewOptions$1', 1);
//Southpaw.Runtime.Clientside.ViewOptions.registerClass('Southpaw.Runtime.Clientside.ViewOptions', Object);

Southpaw.Runtime.Clientside.Router = function (options) {
    this._eventUtils = new Southpaw.Runtime.Clientside.EventUtils();
    options || (options = {});
    if (options.routes)
        this.routes = options.routes;
    this._bindRoutes();
    this.initialize.apply(this, arguments);
};

// TODO: global NS
var namedParam    = /:\w+/g;
var splatParam    = /\*\w+/g;
var escapeRegExp  = /[-[\]{}()+?.,\\^$|#\s]/g;
Southpaw.Runtime.Clientside.Router.prototype = {
    initialize: function(){},

    // Manually bind a single named route to a callback. For example:
    //
    //     this.route('search/:query/p:num', 'search', function(query, num) {
    //       ...
    //     });
    //
    route: function(route, name, callback) {
      Backbone.history || (Backbone.history = new Backbone.History2);
      route = this._routeToRegExp(route);
      if (!callback) callback = this[name];
      Backbone.history.route(route, _.bind(function(fragment) {
        var args = this._extractParameters(route, fragment);
        callback && callback.apply(this, args);
        this.trigger.apply(this, ['route:' + name].concat(args));
        // TODO NCU Backbone.history.trigger('route', this, name, args);
      }, this));
      return this;
    },

    // Simple proxy to `Backbone.history` to save a fragment into the history.
    navigate: function(fragment, options) {
      Backbone.history.navigate(fragment, options);
    },
    getCurrentHash: function() {
      return Backbone.history.getHash();
    },
    startHistory: function() {
        Backbone.history.start();
    },
    stopHistory: function() {
        Backbone.history.stop();
    },

    // Bind all defined routes to `Backbone.history`. We have to reverse the
    // order of the routes here to support behavior where the most general
    // routes can be defined at the bottom of the route map.
    _bindRoutes: function() {
      if (!this.routes) return;
      var routes = [];
      for (var route in this.routes) {
        routes.unshift([route, this.routes[route]]);
      }
      for (var i = 0, l = routes.length; i < l; i++) {
        this.route(routes[i][0], routes[i][1], this[routes[i][1]]);
      }
    },

    // Convert a route string into a regular expression, suitable for matching
    // against the current location hash.
    _routeToRegExp: function(route) {
      route = route.replace(escapeRegExp, '\\$&')
                   .replace(namedParam, '([^\/]+)')
                   .replace(splatParam, '(.*?)');
      return new RegExp('^' + route + '$');
    },

    // Given a route, and a URL fragment that it matches, return the array of
    // extracted parameters.
    _extractParameters: function(route, fragment) {
      return route.exec(fragment).slice(1);
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
(function(window) {
    // Backbone.History
    // ----------------
    if (window.Backbone && window.Backbone.History2)
        return; // already defined

    window.Backbone = window.Backbone || {};
    var $ = window.$;
    var _ = window._;
    var document = window.document;

    // Handles cross-browser history management, based on URL fragments. If the
    // browser does not support `onhashchange`, falls back to polling.
    var History2 = window.Backbone.History2 = function() {
        this.handlers = [];
        _.bindAll(this, 'checkUrl');
    };

    // Cached regex for cleaning leading hashes and slashes .
    var routeStripper = /^[#\/]/;

    // Cached regex for detecting MSIE.
    var isExplorer = /msie [\w.]+/;

    // Has the history handling already been started?
    History2.started = false;

    // Set up all inheritable **Backbone.History** properties and methods.
    _.extend(History2.prototype, Southpaw.Runtime.Clientside.EventUtils, {
        // The default interval to poll for hash changes, if necessary, is
        // twenty times a second.
        interval: 50,

        // Gets the true hash value. Cannot use location.hash directly due to bug
        // in Firefox where location.hash will always be decoded.
        getHash: function(windowOverride) {
            var loc = windowOverride ? windowOverride.location : window.location;
            var match = loc.href.match(/#(.*)$/);
            return match ? match[1] : '';
        },

        // Get the cross-browser normalized URL fragment, either from the URL,
        // the hash, or the override.
        getFragment: function(fragment, forcePushState) {
            if (fragment == null) {
                if (this._hasPushState || forcePushState) {
                    fragment = window.location.pathname;
                    var search = window.location.search;
                    if (search) fragment += search;
                } else {
                    fragment = this.getHash();
                }
            }
            if (!fragment.indexOf(this.options.root)) fragment = fragment.substr(this.options.root.length);
            return fragment.replace(routeStripper, '');
        },

        // Start the hash change handling, returning `true` if the current URL matches
        // an existing route, and `false` otherwise.
        start: function(options) {
            if (History2.started) throw new Error("Backbone.history has already been started");
            History2.started = true;

            // Figure out the initial configuration. Do we need an iframe?
            // Is pushState desired ... is it available?
            this.options = _.extend({ }, { root: '/' }, this.options, options);
            this._wantsHashChange = this.options.hashChange !== false;
            this._wantsPushState = !!this.options.pushState;
            this._hasPushState = !!(this.options.pushState && window.history && window.history.pushState);
            var fragment = this.getFragment();
            var docMode = document.documentMode;
            var oldIE = (isExplorer.exec(navigator.userAgent.toLowerCase()) && (!docMode || docMode <= 7));

            if (oldIE) {
                this.iframe = $('<iframe src="javascript:0" tabindex="-1" />').hide().appendTo('body')[0].contentWindow;
                this.navigate(fragment);
            }

            // Depending on whether we're using pushState or hashes, and whether
            // 'onhashchange' is supported, determine how we check the URL state.
            if (this._hasPushState) {
                $(window).bind('popstate', this.checkUrl);
            } else if (this._wantsHashChange && ('onhashchange' in window) && !oldIE) {
                $(window).bind('hashchange', this.checkUrl);
            } else if (this._wantsHashChange) {
                this._checkUrlInterval = setInterval(this.checkUrl, this.interval);
            }

            // Determine if we need to change the base url, for a pushState link
            // opened by a non-pushState browser.
            this.fragment = fragment;
            var loc = window.location;
            var atRoot = loc.pathname == this.options.root;

            // If we've started off with a route from a `pushState`-enabled browser,
            // but we're currently in a browser that doesn't support it...
            if (this._wantsHashChange && this._wantsPushState && !this._hasPushState && !atRoot) {
                this.fragment = this.getFragment(null, true);
                window.location.replace(this.options.root + '#' + this.fragment);
                // Return immediately as browser will do redirect to new url
                return true;

                // Or if we've started out with a hash-based route, but we're currently
                // in a browser where it could be `pushState`-based instead...
            } else if (this._wantsPushState && this._hasPushState && atRoot && loc.hash) {
                this.fragment = this.getHash().replace(routeStripper, '');
                window.history.replaceState({ }, document.title, loc.protocol + '//' + loc.host + this.options.root + this.fragment);
            }

            if (!this.options.silent) {
                return this.loadUrl();
            }
        },

        // Disable Backbone.history, perhaps temporarily. Not useful in a real app,
        // but possibly useful for unit testing Routers.
        stop: function() {
            $(window).unbind('popstate', this.checkUrl).unbind('hashchange', this.checkUrl);
            window.clearInterval(this._checkUrlInterval);
            History2.started = false;
        },

        // Add a route to be tested when the fragment changes. Routes added later
        // may override previous routes.
        route: function(route, callback) {
            this.handlers.unshift({ route: route, callback: callback });
        },

        // Checks the current URL to see if it has changed, and if it has,
        // calls `loadUrl`, normalizing across the hidden iframe.
        checkUrl: function(e) {
            var current = this.getFragment();
            if (current == this.fragment && this.iframe) current = this.getFragment(this.getHash(this.iframe));
            if (current == this.fragment) return false;
            if (this.iframe) this.navigate(current);
            this.loadUrl() || this.loadUrl(this.getHash());
        },

        // Attempt to load the current URL fragment. If a route succeeds with a
        // match, returns `true`. If no defined routes matches the fragment,
        // returns `false`.
        loadUrl: function(fragmentOverride) {
            var fragment = this.fragment = this.getFragment(fragmentOverride);
            var matched = _.any(this.handlers, function(handler) {
                if (handler.route.test(fragment)) {
                    handler.callback(fragment);
                    return true;
                }
            });
            return matched;
        },

        // Save a fragment into the hash history, or replace the URL state if the
        // 'replace' option is passed. You are responsible for properly URL-encoding
        // the fragment in advance.
        //
        // The options object can contain `trigger: true` if you wish to have the
        // route callback be fired (not usually desirable), or `replace: true`, if
        // you wish to modify the current URL without adding an entry to the history.
        navigate: function(fragment, options) {
            if (!History2.started) return false;
            if (!options || options === true) options = { trigger: options };
            var frag = (fragment || '').replace(routeStripper, '');
            if (this.fragment == frag) return;

            // If pushState is available, we use it to set the fragment as a real URL.
            if (this._hasPushState) {
                if (frag.indexOf(this.options.root) != 0) frag = this.options.root + frag;
                this.fragment = frag;
                window.history[options.replace ? 'replaceState' : 'pushState']({ }, document.title, frag);

                // If hash changes haven't been explicitly disabled, update the hash
                // fragment to store history.
            } else if (this._wantsHashChange) {
                this.fragment = frag;
                this._updateHash(window.location, frag, options.replace);
                if (this.iframe && (frag != this.getFragment(this.getHash(this.iframe)))) {
                    // Opening and closing the iframe tricks IE7 and earlier to push a history entry on hash-tag change.
                    // When replace is true, we don't want this.
                    if (!options.replace) this.iframe.document.open().close();
                    this._updateHash(this.iframe.location, frag, options.replace);
                }

                // If you've told us that you explicitly don't want fallback hashchange-
                // based history, then `navigate` becomes a page refresh.
            } else {
                window.location.assign(this.options.root + fragment);
            }
            if (options.trigger) this.loadUrl(fragment);
        },

        // Update the hash location, either replacing the current entry, or adding
        // a new one to the browser history.
        _updateHash: function(location, fragment, replace) {
            if (replace) {
                location.replace(location.toString().replace(/(javascript:|#).*$/, '') + '#' + fragment);
            } else {
                location.hash = fragment;
            }
        }
    });
})(window);


Southpaw.Runtime.Clientside.Validation = {};
Southpaw.Runtime.Clientside.Validation.Type = {};

Southpaw.Runtime.Clientside.Validation.RangeValidator = function() { };
Southpaw.Runtime.Clientside.Validation.RegexValidator = function () { };
Southpaw.Runtime.Clientside.Validation.RequiredValidator = function () { };
Southpaw.Runtime.Clientside.Validation.LengthValidator = function () { };
Southpaw.Runtime.Clientside.Validation.Type.IntValidator = function () { };
Southpaw.Runtime.Clientside.Validation.Type.DateValidator = function () { };
Southpaw.Runtime.Clientside.Validation.Type.FloatValidator = function () { };
Southpaw.Runtime.Clientside.Validation.Type.BoolValidator = function () { };
Southpaw.Runtime.Clientside.Validation.Type.CharValidator = function () { };

Southpaw.Runtime.Clientside.TypeConverters = {};
Southpaw.Runtime.Clientside.TypeConverters.toInt = function(newValue) {
    if (newValue === null || newValue === undefined)
        return null;
    var x = parseInt(newValue);
    if (isNaN(x))
        return null;
    return x;
};
Southpaw.Runtime.Clientside.TypeConverters.toFloat = function(newValue) {
    if (newValue === null || newValue === undefined)
        return null;
    var x = parseFloat(newValue);
    if (isNaN(x))
        return null;
    return x;
};
Southpaw.Runtime.Clientside.TypeConverters.toBool = function (newValue) {
    if (newValue === null || newValue === undefined)
        return null;
    var n = newValue.toLowerCase();
    if (n == 'true' || n == '1')
        return true;
    if (n == 'false' || n == '0')
        return false;
    return null;
};
Southpaw.Runtime.Clientside.TypeConverters.toChar = function (newValue) {
    if (newValue && newValue.length == 1)
        return newValue;
    return null;
};
Southpaw.Runtime.Clientside.TypeConverters.toDate = function(newValue) {
    if (newValue === null || newValue === undefined)
        return null;
    var n = newValue.split('/');
    if (n.length == 3
        && (n[0].length == 1 || n[0].length == 2)
        && (n[1].length == 1 || n[0].length == 2)
        && (n[2].length == 4)) {
        var d = parseInt(n[0]),
            m = parseInt(n[1]),
            y = parseInt(n[2]);
        n = new Date(y, m, d);
        return n;
    }
    return null;
};

(function () {
    var formatValidationMessage = function (msg, params) {
        if (params.errorMessage !== null && params.errorMessage !== undefined)
            msg = params.errorMessage;
        else
            msg = Southpaw.Runtime.Clientside.GlobalSettings.messageProvider.getMessage(msg);
        for (var key in params) {
            if (!params.hasOwnProperty(key))
                continue;
            var r = '{' + key + '}';
            var idx = msg.indexOf(r);
            if (idx == -1)
                continue;
            msg = msg.substr(0, idx) + params[key] + msg.substr(idx + r.length);
        }
        return msg;
    };
    
    /*
    example call:
    new Southpaw...RangeValidator().validate(3, {
        minimum: 0, 
        maximum: 100,
        property: 'investment percent',
        errorMessage: 'unfortunately, that\'s the incorrect answer' });
    */
    Southpaw.Runtime.Clientside.Validation.RangeValidator.prototype = {
        validate: function(obj, params) {
            if (obj === null || obj === undefined)
                return null;
            if (obj < params.minimum || obj > params.maximum)
                return formatValidationMessage("Southpaw.Runtime.Clientside.Validation.RangeValidator", params);
            return null;
        }
    };
    
    Southpaw.Runtime.Clientside.Validation.RegexValidator.prototype = {
        validate: function (obj, params) {
            if (obj === null || obj === undefined)
                return null;
            if (!params.pattern.test(obj))
                return formatValidationMessage("Southpaw.Runtime.Clientside.Validation.RegexValidator", params);
            return null;
        }
    };
    
    Southpaw.Runtime.Clientside.Validation.RequiredValidator.prototype = {
        validate: function(obj, params) {
            if (obj === null || obj === undefined || (!params["AllowEmptyStrings"] && obj ===""))
                return formatValidationMessage("Southpaw.Runtime.Clientside.Validation.RequiredValidator", params);
            return null;
        }
    };
    
    Southpaw.Runtime.Clientside.Validation.LengthValidator.prototype = {
        validate: function(obj, params) {
            if (obj === null || obj === undefined)
                return null;
            if ((params.minimumLength === null || params.minimumLength === undefined)
                && obj.length > params.maximumLength)
                return formatValidationMessage("Southpaw.Runtime.Clientside.Validation.LengthValidator_MaxOnly", params);
            if ((params.maximumLength === null || params.maximumLength === undefined)
                && obj.length < params.minimumLength)
                return formatValidationMessage("Southpaw.Runtime.Clientside.Validation.LengthValidator_MinOnly", params);
            if (obj.length < params.minimumLength || obj.length > params.maximumLength)
                return formatValidationMessage("Southpaw.Runtime.Clientside.Validation.LengthValidator", params);
            return null;
        }
    };
    Southpaw.Runtime.Clientside.Validation.Type.IntValidator.prototype = {
        validate: function (obj, params) {
            if (obj === null || obj === undefined)
                return null;
            if (Southpaw.Runtime.Clientside.TypeConverters.toInt(obj) == null)
                return formatValidationMessage("Southpaw.Runtime.Clientside.Validation.Type.IntValidator", params);
            return null;
        }
    };
    Southpaw.Runtime.Clientside.Validation.Type.FloatValidator.prototype = {
        validate: function (obj, params) {
            if (obj === null || obj === undefined)
                return null;
            if (Southpaw.Runtime.Clientside.TypeConverters.toFloat(obj) == null)
                return formatValidationMessage("Southpaw.Runtime.Clientside.Validation.Type.FloatValidator", params);
            return null;
        }
    };
    Southpaw.Runtime.Clientside.Validation.Type.BoolValidator.prototype = {
        validate: function (obj, params) {
            if (obj === null || obj === undefined || obj === true || obj === false)
                return null;
            if (Southpaw.Runtime.Clientside.TypeConverters.toBool(obj) == null)
                return formatValidationMessage("Southpaw.Runtime.Clientside.Validation.Type.BoolValidator", params);
            return null;
        }
    };
    Southpaw.Runtime.Clientside.Validation.Type.DateValidator.prototype = {
        validate: function (obj, params) {
            if (obj === null || obj === undefined || (obj.toUTCString && obj.toDateString))
                return null;
            if (Southpaw.Runtime.Clientside.TypeConverters.toDate(obj) == null)
                return formatValidationMessage("Southpaw.Runtime.Clientside.Validation.Type.DateValidator", params);
            return null;
        }
    };
    Southpaw.Runtime.Clientside.Validation.Type.CharValidator.prototype = {
        validate: function(obj, params) {
            if (obj === null || obj === undefined)
                return null;
            if (Southpaw.Runtime.Clientside.TypeConverters.toChar(obj) == null)
                return formatValidationMessage("Southpaw.Runtime.Clientside.Validation.Type.CharValidator", params);
            return null;
        }
    };
})();

Southpaw.Runtime.Clientside.Messages = {
    "Southpaw.Runtime.Clientside.Validation.RangeValidator": "{property} should be between {minimum} and {maximum}",
    "Southpaw.Runtime.Clientside.Validation.RegexValidator": "{property} is not valid",
    "Southpaw.Runtime.Clientside.Validation.RequiredValidator": "{property} is required",
    "Southpaw.Runtime.Clientside.Validation.LengthValidator": "{property} should be between {minimumLength} and {maximumLength} characters long",
    "Southpaw.Runtime.Clientside.Validation.LengthValidator_MinOnly": "{property} should be at least {minimumLength} characters long",
    "Southpaw.Runtime.Clientside.Validation.LengthValidator_MaxOnly": "{property} should be at most {maximumLength} characters long",
    "Southpaw.Runtime.Clientside.Validation.Type.IntValidator": "{property} should be a valid number",
    "Southpaw.Runtime.Clientside.Validation.Type.DateValidator": "{property} should be a valid date",
    "Southpaw.Runtime.Clientside.Validation.Type.FloatValidator": "{property} should be a valid decimal number",
    "Southpaw.Runtime.Clientside.Validation.Type.BoolValidator": "{property} should be 'true' or 'false'",
    "Southpaw.Runtime.Clientside.Validation.Type.CharValidator": "{property} should be a single character",
};
Southpaw.Runtime.Clientside.MessageProvider = function() {
};

Southpaw.Runtime.Clientside.MessageProvider.prototype.getMessage = function (key) {
    var msg = Southpaw.Runtime.Clientside.Messages[key];
    if (msg == undefined)
        throw 'No message defined for key "' + key + '"';
    return msg;
};
Southpaw.Runtime.Clientside.GlobalSettings.messageProvider = new Southpaw.Runtime.Clientside.MessageProvider();