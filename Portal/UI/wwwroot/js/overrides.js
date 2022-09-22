const noop = function () { };
const console = { };

console.info = noop;
console.log = noop;
console.debug = noop;
console.error = noop;
console.warning = noop;

window.console = console;