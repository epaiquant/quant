'use strict'
const mysql = require('./mysqldb')
var Promise = require('bluebird')
var co = require('co')
var util = require('../common/util')
var Msg = require('../common/Msg')

var Strategies = {}
Strategies.FindOne = function (userId, strategyId) {
    return new Promise(function (resolve, reject) {
        co(function* () {
            const sql = "SELECT * from strategies where UserId = ? and StrategyId = ?"
            mysql.db.query(sql, [userId, strategyId], function (err, result) {
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

Strategies.FindAllStrategies = function (userId) {
    return new Promise(function (resolve, reject) {
        co(function* () {
            const sql = "SELECT StrategyId,`Name`,Category from strategies where UserId = ? order by Category,`Name`"
            mysql.db.query(sql, [userId], function (err, result) {
                if (err) {
                    reject(err)
                } else {
                    if (result.length > 0) {
                        resolve(Msg.Success(result))
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

Strategies.Add = function (strategy) {
    return new Promise(function (resolve, reject) {
        co(function* () {
            var strategyId = util.GetUUID()
            const sql = "insert into strategies(StrategyId,UserId,`Name`,Category,Code,StrategyParams,StrategySettings,StrategyWorker,RealAccount,IsAuto) " 
                + "values(?,?,?,?,?,?,?,?,?,?)"
            mysql.db.query(sql, [
                strategyId,
                strategy.UserId,
                strategy.Name,
                strategy.Category,
                strategy.Code,
                strategy.StrategyParams,
                strategy.StrategySettings,
                strategy.StrategyWorker,
                strategy.RealAccount,
                strategy.IsAuto
            ], function (err, result) {
                if (err) {
                    reject(err)
                } else {
                    if (result && result.affectedRows > 0) {
                        resolve(Msg.Success({ Id: strategyId }))
                    } else {
                        reject(Msg.Err.DBError)
                    }
                }
            })
        }).catch(function (err) {
            reject(Msg.Error(err))
        })
    })
}

Strategies.Delete = function (userId, strategyId) {
    return new Promise(function (resolve, reject) {
        co(function* () {
            const sql = "delete from strategies where StrategyId=? and UserId=?"
            mysql.db.query(sql, [strategyId, userId], function (err, result) {
                if (err) {
                    reject(Msg.Error(err))
                } else {
                    if (result && result.affectedRows > 0) {
                        resolve(Msg.Info.DBSuccess)
                    } else {
                        resolve(Msg.Err.DBError)
                    }
                }
            })
        }).catch(function (err) {
            reject(Msg.Error(err))
        })
    })
}

Strategies.Update = function (strategy) {
    return new Promise(function (resolve, reject) {
        co(function* () {
            const sql = "update strategies set `Name`=?,Category=?,Code=?,StrategyParams=?,StrategySettings=?,StrategyWorker=?,RealAccount=?,IsAuto=? "
                + "where UserId=? and StrategyId=?"
            mysql.db.query(sql, [
                strategy.Name,
                strategy.Category,
                strategy.Code,
                strategy.StrategyParams,
                strategy.StrategySettings,
                strategy.StrategyWorker,
                strategy.RealAccount,
                strategy.IsAuto,
                strategy.UserId,
                strategy.StrategyId
            ], function (err, result) {
                if (err) {
                    reject(Msg.Error(err))
                } else {
                    if (result && result.affectedRows > 0) {
                        resolve(Msg.Info.DBSuccess)
                    } else {
                        resolve(Msg.Err.DBError)
                    }
                }
            })
        }).catch(function (err) {
            reject(Msg.Error(err))
        })
    })
}

module.exports = Strategies