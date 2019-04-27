// 加密字符串
function encode(encodeStr) {
    return window.btoa(pako.gzip(encodeURIComponent(encodeStr), { to: "string" }));
}
// 解密字符串
function decode(decodeStr) {
    var decodedData = window.atob(decodeStr);
    var charData = decodedData.split('').map(function (x) { return x.charCodeAt(0); });
    var binData = new Uint8Array(charData);
    var data = pako.inflate(binData);
    return decodeURIComponent(String.fromCharCode.apply(null, new Uint16Array(data)));
}