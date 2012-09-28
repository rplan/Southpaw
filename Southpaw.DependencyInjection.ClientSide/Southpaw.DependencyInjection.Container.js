(function(env) {
    var di = function() {
        var dict = { };
        return {
            r: function(target, implementation) {
                dict[target] = implementation;
            },
            g: function(target) {
                var ctor = dict[target];
                if (ctor) {
                    if (arguments.length == 1)
                        return new ctor();
                    var tmp = function() {};
                    tmp.prototype = ctor.prototype;
                    var inst = new tmp;
                    var ret = ctor.apply(inst, Array.prototype.slice.call(arguments, 1));
                    return Object(ret) === ret ? ret : inst;
                }
                throw "No implementions found for " + target.get_fullName();
            }
        };
    };
    env.$DI = new di();
})(window);
