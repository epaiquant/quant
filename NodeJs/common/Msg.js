'use strict'
var Err = {}
var Info = {}

Err.ServerError = { status: false, code: 9001, msg: 'Server Error' }
Err.DBError = { false: false, code: 9002, msg: 'DataBase Error' }
Err.ParamError = { false: false, code: 9003, msg: 'Params Error'}
Err.LoginFaild = { false: false, code: 9103, msg: 'Login Failed' }
Err.NoAccess = { false: false, code: 9104, msg: 'No Access' }

Info.Success = { status: true, code: 1001, msg: 'Success' }


function Success(obj) {
    return { status: true, result: obj}
}
function Error(obj) {
    return { status: false, result: obj }
}
module.exports = { Err, Info, Success, Error }