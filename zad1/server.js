let app = require('express')();
let http = require('http').Server(app);
let io = require('socket.io')(http);
let users = [];

app.get('/', (req, res) => {
    res.sendFile(__dirname + '/index.html')
});

http.listen(3000, () => {
    console.log('Listening on port *: 3000');
});

io.on('connection', (socket) => {

    socket.on('disconnect', () => {
        console.log("A user disconnected");
    });

    socket.on('chat-message', (data) => {
        socket.broadcast.emit('chat-message', (data));
    });

    socket.on('typing', (data) => {
        socket.broadcast.emit('typing', (data));
    });

    socket.on('stopTyping', () => {
        socket.broadcast.emit('stopTyping');
    });
	
	socket.on('requestUsers', () => {
        socket.emit('getUsers', (users));
    });

    socket.on('joinToChat', (data) => {
		
		if (users.find(u => u.name == data.name && u.password == data.password)){
			
			socket.emit('completionStatusReturn', {mode: "authenticatedMode", status:"Успешный вход."});
			socket.broadcast.emit('joined', (data));	
			
			} else {				
				socket.emit('completionStatusReturn', {mode: "loginMode", status:"Имя пользователя или пароль неверны. Попробуйте еще."});			
			}				
			
    });
	
	socket.on('addUser', (data) => {    
		if (users.find(u => u.name == data.name)) {
			socket.emit('completionStatusReturn', {mode: "signinMode", status:"Ошибка: пользователь уже создан."});
		} else {
			users.push(data);
			socket.emit('completionStatusReturn', {mode: "loginMode", status:"Пользователь успешно создан! Вы можете войти."});
			socket.broadcast.emit('getUsers', (users));
		}
    });
});