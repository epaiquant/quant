var express = require('express')
var router = express.Router()
var co = require('co')
const Users = require('../models/Users')
const Msg = require('../common/Msg')
const util = require('../common/util')

router.post('/login', function (req, res, next) {
    if (req.session.UserId) {
        res.json(Msg.Info.IsLogined)
    } else {
        co(function* () {
            if (req.body.userName && req.body.userPwd) {
                let r1 = yield Users.Login(req.body.userName, req.body.userPwd)
                req.session.regenerate(function (err) {
                    if (err) {
                        res.json(Msg.Error(err))
                    }
                    if (r1.status) {
                        req.session.UserId = r1.result.UserId;
                    }
                    res.json(r1)
                })
            } else {
                res.json(Msg.Err.ParamError)
            }
        }).catch(function (err) {
            res.json(Msg.Error(err))
        })
    }
});

router.get('/logout', function (req, res, next) {
    req.session.destroy(function (err) {
        if (err) {
            res.json(Msg.Error(err))
            return
        }
        if (req.session) {
            res.clearCookie(req.session.UserId)
            req.session.UserId = null
        }
        res.redirect('../../login')
    })
})

router.get('/info', util.DoAuth, function (req, res, next) {
    co(function* () {
        if (req.query.id) {
            let user = yield Users.FindOne(req.query.id)
            res.json(user)
        } else {
            res.json(Msg.Err.NoAccess) 
        }
    }).catch(function (err) {
        res.json(Msg.ServerError)
    })
});

module.exports = router
