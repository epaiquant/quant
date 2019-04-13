var express = require('express')
var router = express.Router()
var co = require('co')
const util = require('../common/util')
const Users = require('../models/Users')
const Msg = require('../common/Msg')

router.get('/', util.DoAuth, function (req, res, next) {
    co(function* () {
        if (req.session.UserId) {
            let r1 = yield Users.FindOne(req.session.UserId)
            res.render('user',{UserName:r1.result.UserName})
        }
    }).catch(function (err) {
        res.json(Msg.Error(err))
    })
})

module.exports = router
