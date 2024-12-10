-- Create the database
CREATE DATABASE SnakeGame;
GO

-- Use the database
USE SnakeGame;
GO

-- Create the users table
CREATE TABLE users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
	role VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);
GO

-- Create the leaderboard table
CREATE TABLE leaderboard (
    score_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    score INT NOT NULL,
    achieved_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE
);
GO

-- Insert sample users
INSERT INTO users (username, password, role, email) 
VALUES 
('admin', '@1', 'admin', 'admin@example.com'),
('player1', '@1' ,'player' , 'player1@example.com'),
('player2', '@1', 'player', 'player2@example.com'),
('player3', '@1', 'player', 'player3@example.com');
GO

-- Insert sample scores
INSERT INTO leaderboard (user_id, score, achieved_at) 
VALUES
(1, 5000, CURRENT_TIMESTAMP), -- Admin's score
(2, 3000, CURRENT_TIMESTAMP), -- Player1's score
(2, 2500, CURRENT_TIMESTAMP), -- Another score for Player1
(3, 2000, CURRENT_TIMESTAMP), -- Player2's score
(4, 1500, CURRENT_TIMESTAMP); -- Player3's score
GO
