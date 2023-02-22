const express = require("express");
const multer  = require("multer");
  
const app = express();

let http = require('http').Server(app);
let io = require('socket.io')(http);
 
io.on('connection', (socket) => {

    socket.on('disconnect', () => {
        console.log("A user disconnected");
    });

});

const storageConfig = multer.diskStorage({
    destination: (req, file, cb) =>{
        cb(null, "uploads");
    },
    filename: (req, file, cb) =>{
        cb(null, file.originalname);
    }
});
 
app.use(function (request, response) {
  response.sendFile(__dirname + "/index.html");
});
 
app.use(multer({storage:storageConfig}).single("filedata"));
app.post("/upload", function (req, res, next) {
   
    let filedata = req.file;
    if(!filedata)
        res.send("Ошибка при загрузке файла");
    else
        res.send("Файл загружен");
});

app.listen(3000, ()=>{console.log("Server started");});