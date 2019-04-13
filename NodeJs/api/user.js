var express = require('express')
var router = express.Router()
var co = require('co')
const Users = require('../models/Users')
const Msg = require('../common/Msg')
const util = require('../common/util')

router.post('/login', function (req, res, next) {
    if (req.session.UserId) {
        res.json(Msg.Info.Success)
    } else {
        co(function* () {
            if (req.body.userName && req.body.userPwd) {
                let r1 = yield Users.Login(req.body.userName, req.body.userPwd)
                req.session.regenerate(function (err) {
                    if (err) {
                        res.json(Msg.ServerError)
                    }
                    if (r1.status) {
                        req.session.UserId = r1.result.UserId;
                    }
                    res.json(r1)
                })
            }
        }).catch(function (err) {
            res.json(Msg.Error(err))
        })
    }
});

router.post('/logout', function (req, res, next) {
    req.session.destroy(function (err) {
        if (err) {
            res.json(Msg.Error(err))
            return
        }
        res.clearCookie(req.session.UserId)
        req.session.UserId = null
        res.redirect('/')
    })
})

router.post('/info', util.DoAuth, function (req, res, next) {
    co(function* () {
        if (req.body.id) {
            let user = yield Users.FindOne(req.body.id)
            res.json(user)
        }
    }).catch(function (err) {
        res.json(Msg.ServerError)
    })
});

module.exports = router
