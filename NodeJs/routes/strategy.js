var express = require('express')
var router = express.Router()
var co = require('co')
const util = require('../common/util')
const Strategies = require('../models/Strategies')
const Msg = require('../common/Msg')

router.get('/', util.DoAuth, function (req, res, next) {
    co(function* () {
        if (req.session.UserId) {
            let r1 = yield Strategies.FindAllStrategies(req.session.UserId)
            res.render('strategy', { Strategies: r1.result })
        }
    }).catch(function (err) {
        res.json(Msg.Error(err))
    })
})

module.exports = router
