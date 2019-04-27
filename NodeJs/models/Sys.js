'use strict'
var Promise = require('bluebird')
var co = require('co')
const mysql = require('./mysqldb')
const Msg = require('../common/Msg')
// 私有方法
function Log(msg, isError) {
    var msgType = isError ? 'Error' : 'Info'
    return new Promise(function (resolve, reject) {
        if (msg) {
            if (typeof msg === typeof {}) {
                msg = JSON.stringify(msg)
            }
            co(function* () {
                const sql = "insert into logs(Category,Message) values(?,?)"
                mysql.db.query(sql, [msgType, msg], function (err, result) {
                    if (err) {
                        reject(Msg.Error(err))
                    } else {
                        resolve(result)
                    }
                })
            }).catch(function (err) {
                reject(Msg.Error(err))
            })
        } else {
            reject(Msg.Err.ParamError)
        }
    })
}
// 公共Sys类
var Sys = {}
// 添加消息日志
Sys.AddLog = function (msg) {
    return Log('info', msg)
}
// 添加错误日志
Sys.AddError = function (msg) {
    return Log('Error', msg)
}
module.exports = Sys