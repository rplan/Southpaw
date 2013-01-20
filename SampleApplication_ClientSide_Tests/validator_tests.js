
module('Validators');

test('RangeValidator should validate range', function() {
    var res = new Southpaw.Runtime.Clientside.Validation.RangeValidator().validate(3, { minimum: 0, maximum: 10, property: 'TestProp' });
    equal(res, null);
    res = new Southpaw.Runtime.Clientside.Validation.RangeValidator().validate(3, { minimum: 5, maximum: 10, property: 'TestProp' });
    equal(res, 'TestProp should be between 5 and 10');
});

test('RegexValidator should validate to pattern', function() {
    var res = new Southpaw.Runtime.Clientside.Validation.RegexValidator().validate(3, { pattern: '[0-9]', property: 'TestProp' });
    equal(res, null);
    res = new Southpaw.Runtime.Clientside.Validation.RegexValidator().validate('hello', { pattern: '[0-9]', property: 'TestProp' });
    equal(res, 'TestProp is not valid');
});
test('LengthValidator should validate string length', function() {
    var res = new Southpaw.Runtime.Clientside.Validation.LengthValidator().validate('hello', { minimumLength: 0, maximumLength: 10, property: 'TestProp' });
    equal(res, null);
    res = new Southpaw.Runtime.Clientside.Validation.LengthValidator().validate('hello', { minimumLength: 8, maximumLength: 10, property: 'TestProp' });
    equal(res, 'TestProp should be between 8 and 10 characters long');
    res = new Southpaw.Runtime.Clientside.Validation.LengthValidator().validate('hello', { minimumLength: 8, property: 'TestProp' });
    equal(res, 'TestProp should be at least 8 characters long');
    res = new Southpaw.Runtime.Clientside.Validation.LengthValidator().validate('hello', { maximumLength: 3, property: 'TestProp' });
    equal(res, 'TestProp should be at most 3 characters long');
});
test('RequiredValidator should validate required values', function() {
    var res = new Southpaw.Runtime.Clientside.Validation.RequiredValidator().validate(3, { property: 'TestProp' });
    equal(res, null);
    res = new Southpaw.Runtime.Clientside.Validation.RequiredValidator().validate(null, { property: 'TestProp' });
    equal(res, 'TestProp is required');
    res = new Southpaw.Runtime.Clientside.Validation.RequiredValidator().validate(undefined, { property: 'TestProp' });
    equal(res, 'TestProp is required');
});

test('Validator should allow custom error messages', function() {
    var res = new Southpaw.Runtime.Clientside.Validation.RequiredValidator().validate(null, { property: 'TestProp', errorMessage: 'test error message' });
    equal(res, 'test error message');
});


module('Type Validators');

test('IntValidator should validate int', function() {
    var res = new Southpaw.Runtime.Clientside.Validation.Type.IntValidator().validate(3, { property: 'TestProp' });
    equal(res, null);
    res = new Southpaw.Runtime.Clientside.Validation.Type.IntValidator().validate('3', { property: 'TestProp' });
    equal(res, null);
    res = new Southpaw.Runtime.Clientside.Validation.Type.IntValidator().validate('hello', { property: 'TestProp' });
    equal(res, 'TestProp should be a valid number');
    res = new Southpaw.Runtime.Clientside.Validation.Type.IntValidator().validate('3.2', { property: 'TestProp' });
    equal(res, 'TestProp should be a valid number');
    res = new Southpaw.Runtime.Clientside.Validation.Type.IntValidator().validate(3.2, { property: 'TestProp' });
    equal(res, 'TestProp should be a valid number');
});

test('FloatValidator should validate float', function() {
    var res = new Southpaw.Runtime.Clientside.Validation.Type.FloatValidator().validate(3, { property: 'TestProp' });
    equal(res, null);
    res = new Southpaw.Runtime.Clientside.Validation.Type.FloatValidator().validate(3.1, { property: 'TestProp' });
    equal(res, null);
    res = new Southpaw.Runtime.Clientside.Validation.Type.FloatValidator().validate('3', { property: 'TestProp' });
    equal(res, null);
    res = new Southpaw.Runtime.Clientside.Validation.Type.FloatValidator().validate('3.1', { property: 'TestProp' });
    equal(res, null);
    res = new Southpaw.Runtime.Clientside.Validation.Type.FloatValidator().validate('hello', { property: 'TestProp' });
    equal(res, 'TestProp should be a valid decimal number');
});


test('DateValidator should validate dates', function() {
    var res = new Southpaw.Runtime.Clientside.Validation.Type.DateValidator().validate(new Date(1,1,1900), { property: 'TestProp' });
    equal(res, null);
    res = new Southpaw.Runtime.Clientside.Validation.Type.DateValidator().validate('1/1/2012', { property: 'TestProp' });
    equal(res, null);
    res = new Southpaw.Runtime.Clientside.Validation.Type.DateValidator().validate('hello', { property: 'TestProp' });
    equal(res, 'TestProp should be a valid date');
});

test('BoolValidator should validate dates', function() {
    var res = new Southpaw.Runtime.Clientside.Validation.Type.BoolValidator().validate(true, { property: 'TestProp' });
    equal(res, null);
    res = new Southpaw.Runtime.Clientside.Validation.Type.BoolValidator().validate(false, { property: 'TestProp' });
    equal(res, null);
    res = new Southpaw.Runtime.Clientside.Validation.Type.BoolValidator().validate('1', { property: 'TestProp' });
    equal(res, null);
    res = new Southpaw.Runtime.Clientside.Validation.Type.BoolValidator().validate('0', { property: 'TestProp' });
    equal(res, null);
    res = new Southpaw.Runtime.Clientside.Validation.Type.BoolValidator().validate('true', { property: 'TestProp' });
    equal(res, null);
    res = new Southpaw.Runtime.Clientside.Validation.Type.BoolValidator().validate('false', { property: 'TestProp' });
    equal(res, null);
    res = new Southpaw.Runtime.Clientside.Validation.Type.BoolValidator().validate('hello', { property: 'TestProp' });
    equal(res, "TestProp should be 'true' or 'false'");
});

test('CharValidator should validate dates', function() {
    var res = new Southpaw.Runtime.Clientside.Validation.Type.CharValidator().validate('a', { property: 'TestProp' });
    equal(res, null);
    res = new Southpaw.Runtime.Clientside.Validation.Type.CharValidator().validate('hello', { property: 'TestProp' });
    equal(res, 'TestProp should be a single character');
});


module(null);
