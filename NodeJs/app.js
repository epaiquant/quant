var createError = require('http-errors')
var express = require('express')
var path = require('path')
var partials = require("express-partials")
var cookieParser = require('cookie-parser')
var logger = require('morgan')
var session = require('express-session');
var FileStore = require('session-file-store')(session);

var indexRouter = require('./routes/index')
var userRouter = require('./routes/user')
var userApi = require('./api/user')

var app = express()
app.use(partials())

app.use(session({
  name: 'quant',
  secret: 'quantkey',  // 用来对session id相关的cookie进行签名
  store: new FileStore(),  // 本地存储session（文本文件，也可以选择其他store，比如redis的）
  saveUninitialized: false,  // 是否自动保存未初始化的会话，建议false
  resave: false,  // 是否每次都重新保存会话，建议false
  cookie: {
    maxAge: 3600 * 1000  // 有效期，单位是毫秒
  }
}))

// view engine setup
app.set('views', path.join(__dirname, 'views'))
app.set('view engine', 'ejs')

app.use(logger('dev'))
app.use(express.json())
app.use(express.urlencoded({ extended: false }))
app.use(cookieParser())
app.use(express.static(path.join(__dirname, 'public')))

app.use('/api/user', userApi)
app.use('/', indexRouter)
app.use('/user', userRouter)

// catch 404 and forward to error handler
app.use(function(req, res, next) {
  next(createError(404))
});

// error handler
app.use(function(err, req, res, next) {
  // set locals, only providing error in development
  res.locals.message = err.message
  res.locals.error = req.app.get('env') === 'development' ? err : {}

  // render the error page
  res.status(err.status || 500)
  res.render('error')
})

module.exports = app
