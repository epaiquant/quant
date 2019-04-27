var express = require('express')
var router = express.Router()
var co = require('co')
const Strategies = require('../models/Strategies')
const Msg = require('../common/Msg')
const util = require('../common/util')

router.get('/index', util.DoAuth, function (req, res, next) {
    co(function* () {
        if (req.query.UserId) {
            let r1 = yield Strategies.FindAllStrategies(req.query.UserId)
            res.json(r1)
        } else {
            res.json(Msg.Err.ParamError)
        }
    }).catch(function (err) {
        res.json(err)
    })
});

router.get('/detail', util.DoAuth, function (req, res, next) {
    co(function* () {
        if (req.query.UserId && req.query.StrategyId) {
            let r1 = yield Strategies.FindOne(req.query.UserId, req.query.StrategyId)
            res.json(r1)
        } else {
            res.json(Msg.Err.ParamError)
        }
    }).catch(function (err) {
        res.json(err)
    })
});

router.delete('/del', util.DoAuth, function (req, res, next) {
    co(function* () {
        if (req.body.StrategyId) {
            let r1 = yield Strategies.Delete(req.body.UserId, req.body.StrategyId)
            res.json(r1)
        } else {
            res.json(Msg.Err.ParamError)
        }
    }).catch(function (err) {
        res.json(err)
    })
});

router.post('/add', util.DoAuth, function (req, res, next) {
    co(function* () {
        if (req.body.UserId && req.body.Name) {
            var strategy = {
                UserId: req.body.UserId,
                Name: req.body.Name,
                Category: (req.body.Category ? req.body.Category : '趋势'),
                Code: req.body.Code,
                StrategyParams: req.body.StrategyParams,
                StrategySettings: req.body.StrategySettings,
                StrategyWorker: req.body.StrategyWorker,
                RealAccount: req.body.RealAccount,
                IsAuto: (req.body.IsAuto ? 1 : 0)
            }
            let r1 = yield Strategies.Add(strategy)
            res.json(r1)
        } else {
            res.json(Msg.Err.ParamError)
        }
    }).catch(function (err) {
        res.json(err)
    })
});

router.put('/update', util.DoAuth, function (req, res, next) {
    co(function* () {
        if (req.body.UserId && req.body.StrategyId && req.body.Name && req.body.Category) {
            var strategy = {
                StrategyId: req.body.StrategyId,
                UserId: req.body.UserId,
                Name: req.body.Name,
                Category: req.body.Category,
                Code: req.body.Code,
                StrategyParams: req.body.StrategyParams,
                StrategySettings: req.body.StrategySettings,
                StrategyWorker: req.body.StrategyWorker,
                RealAccount: req.body.RealAccount,
                IsAuto: req.body.IsAuto
            }
            let r1 = yield Strategies.Update(strategy)
            res.json(r1)
        } else {
            res.json(Msg.Err.ParamError)
        }
    }).catch(function (err) {
        res.json(err)
    })
});

module.exports = router
