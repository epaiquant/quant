'use strict'

const mysql = require('mysql')
var mysqldb = {}

function connectDB() {
    mysqldb.db = mysql.createConnection({
        host: '127.0.0.1',
        user: 'root',
        password: '123456',
        port: '3306',
        database: 'quant',
        multipleStatements: true
    })
    console.log("==========>>>>>mysql connect<<<<<<===========")
    // 连接错误，2秒重试
    mysqldb.db.connect(function (err) {
        if (err) {
            console.log('error when connecting to db:', err)
            setTimeout(handleError, 2000)
        } else {
            console.log('mysql connect ok!')
        }
    })
    mysqldb.db.on('error', function (err) {
        console.log('db error', err)
        // 如果是连接断开，自动重新连接
        if (err.code == 'PROTOCOL_CONNECTION_LOST' || err.code == 'ECONNREFUSED') {
            connectDB()
        } else {
            console.log(err)
        }
    })
}
connectDB()

module.exports = mysqldb
