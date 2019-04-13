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
                console.log(decoded)
                // 此处可以对token信息进行验证和权限控制
                next()
            } catch (err) {
                return next()
            }
        } else {
            res.json(Msg.Err.NoAccess)
        }
    }
}


module.exports = { GetJwtToken, DoAuth }