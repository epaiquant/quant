'use strict'
const mysql = require('./mysqldb')
var Promise = require('bluebird')
var co = require('co')
var util = require('../common/util')
var Msg = require('../common/Msg')

var Users = {}
Users.FindOne = function (id) {
    return new Promise(function (resolve, reject) {
        co(function* () {
            const sql = "select * from users where UserId = ?"
            mysql.db.query(sql, [id], function (err, result) {
                if (err) {
                    reject(err)
                } else {
                    if (result.length > 0) {
                        resolve(Msg.Success(result[0]))
                    } else {
                        resolve(Msg.Err.LoginFaild)
                    }
                }
            })
        }).catch(function (err) {
            reject(err)
        })
    })
}
// 用户登录
Users.Login = function (userName, userPwd) {
    return new Promise(function (resolve, reject) {
        co(function* () {
            const sql = "select * from users where UserName = ? and UserPwd = ?"
            mysql.db.query(sql, [userName, userPwd], function (err, result) {
                if (err) {
                    reject(err)
                } else {
                    if (result.length > 0) {
                        var user = result[0]
                        var token = util.GetJwtToken(user.UserId)
                        resolve(Msg.Success({ Token: token, UserId: user.UserId, UserName: user.UserName }))
                    } else {
                        resolve(Msg.Err.LoginFaild)
                    }
                }
            })
        }).catch(function (err) {
            reject(err)
        })
    })
}
module.exports = Users