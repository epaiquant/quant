'use strict'

var jwt = require('jwt-simple')
var Buffer = require('Buffer')
const Msg = require('../common/Msg')
// var moment = require('moment')
const secret = Buffer.from('epiquantsecret', 'hex')

// 获取Token
function GetJwtToken(userId) {
    //var expires = moment().add(7,'days').valueOf()
    //console.log(expires)
    var payload = { Id: userId };
    var token = jwt.encode(payload, secret)
    console.log(token)
    return token
}

// 验证Token
function DoAuth(req, res, next) {
    if (req.session.UserId) {
        next()
    } else {
        var token = (req.body && req.body.access_token) || (req.query && req.query.access_token) 
            || req.header['x-access-token']
        if (token) {
            try {
                var decoded = jwt.decode(token, secret);
                // 此处可以对token信息进行验证和权限控制
                next()
            } catch (err) {
                return next()
            }
        } else {
            if (req.originalUrl.indexOf('/api/') >= 0) {
                res.json(Msg.Err.NoAccess)
            } else {
                res.render('login')
            }
        }
    }
}
// 获取UUID
function GetUUID() {
    var s = [];
    var hexDigits = "0123456789abcdef";
    for (var i = 0; i < 36; i++) {
        s[i] = hexDigits.substr(Math.floor(Math.random() * 0x10), 1);
    }
    s[14] = "4";  // bits 12-15 of the time_hi_and_version field to 0010
    s[19] = hexDigits.substr((s[19] & 0x3) | 0x8, 1);  // bits 6-7 of the clock_seq_hi_and_reserved to 01
    s[8] = s[13] = s[18] = s[23] = "-";

    var uuid = s.join("");
    return uuid;
}


module.exports = { GetJwtToken, DoAuth, GetUUID }