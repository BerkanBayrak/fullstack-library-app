CREATE DATABASE IF NOT EXISTS `librarydb` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `librarydb`;

DROP TABLE IF EXISTS `Books`;
DROP TABLE IF EXISTS `Shelves`;
DROP TABLE IF EXISTS `Bookshelves`;

CREATE TABLE `Bookshelves` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Label` VARCHAR(64) NOT NULL,
  `Height` INT NOT NULL,
  `Width` INT NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_Bookshelves_Label` (`Label`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `Shelves` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Height` INT NOT NULL,
  `Width` INT NOT NULL,
  `BookshelfId` INT NOT NULL,
  `Position` INT NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Shelves_BookshelfId` (`BookshelfId`),
  CONSTRAINT `FK_Shelves_Bookshelves_BookshelfId`
    FOREIGN KEY (`BookshelfId`) REFERENCES `Bookshelves`(`Id`)
    ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `Books` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Title` VARCHAR(255) NOT NULL,
  `Height` INT NOT NULL,
  `Width` INT NOT NULL,
  `Author` VARCHAR(255) NOT NULL,
  `Genre` VARCHAR(128) NOT NULL,
  `ShelfId` INT NULL,
  `Type` INT NOT NULL,
  `Status` INT NOT NULL,
  `BorrowerId` VARCHAR(64) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Books_ShelfId` (`ShelfId`),
  CONSTRAINT `FK_Books_Shelves_ShelfId`
    FOREIGN KEY (`ShelfId`) REFERENCES `Shelves`(`Id`)
    ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `Bookshelves` (`Id`,`Label`,`Height`,`Width`) VALUES
(21,'A',100,100),
(22,'B',100,100),
(23,'C',150,100);

INSERT INTO `Shelves` (`Id`,`Height`,`Width`,`BookshelfId`,`Position`) VALUES
(160,25,100,21,1),
(161,25,100,21,2),
(162,25,100,21,3),
(163,25,100,21,4),
(164,25,100,22,1),
(165,25,100,22,2),
(166,25,100,22,3),
(167,25,100,22,4),
(168,25,100,23,1),
(169,25,100,23,2),
(170,25,100,23,3),
(171,25,100,23,4),
(172,25,100,23,5),
(173,25,100,23,6);

INSERT INTO `Books`
(`Id`,`Title`,`Height`,`Width`,`Author`,`Genre`,`ShelfId`,`Type`,`Status`,`BorrowerId`) VALUES
(9,'1984',19,3,'George Orwell','Dystopian',NULL,0,2,''),
(10,'To Kill a Mockingbird',21,3,'Harper Lee','Classic',160,0,1,''),
(11,'A Brief History of Time',22,2,'Stephen Hawking','Science',160,3,1,''),
(12,'The Hobbit',19,3,'J.R.R. Tolkien','Fantasy',160,0,1,''),
(13,'The Catcher in the Rye',18,2,'J.D. Salinger','Coming-of-Age',160,0,1,''),
(15,'The Art of War',18,2,'Sun Tzu','Philosophy',160,3,1,''),
(16,'Sapiens: A Brief History of Humankind',23,4,'Yuval Noah Harari','History',160,4,1,''),
(17,'Harry Potter and the Sorcerer’s Stone',22,3,'J.K. Rowling','Fantasy',160,0,1,''),
(18,'The Name of the Wind',24,4,'Patrick Rothfuss','Fantasy',160,0,1,''),
(19,'Thinking, Fast and Slow',21,3,'Daniel Kahneman','Psychology',160,4,1,''),
(20,'Dune',23,4,'Frank Herbert','Sci-Fi',160,0,1,''),
(21,'The Brothers Karamazov',25,4,'Fyodor Dostoevsky','Classic',160,0,1,''),
(22,'The Subtle Art of Not Giving a F*ck',20,2,'Mark Manson','Self-Help',160,5,1,''),
(23,'The Lean Startup',21,2,'Eric Ries','Business',160,4,1,''),
(24,'The Alchemist',20,2,'Paulo Coelho','Fiction',160,0,1,''),
(25,'Inferno',22,3,'Dan Brown','Thriller',160,0,1,''),
(26,'The Shining',24,4,'Stephen King','Horror',160,0,1,''),
(27,'The Road',21,2,'Cormac McCarthy','Post-Apocalyptic',160,0,1,''),
(28,'Rich Dad Poor Dad',20,2,'Robert Kiyosaki','Finance',160,4,1,''),
(29,'The Power of Habit',22,3,'Charles Duhigg','Psychology',160,4,1,''),
(30,'Brave New World',21,2,'Aldous Huxley','Dystopian',160,0,1,''),
(31,'The Prince',18,2,'Niccolò Machiavelli','Political',160,3,1,''),
(32,'Man''s Search for Meaning',20,2,'Viktor Frankl','Philosophy',160,3,1,''),
(33,'The Theory of Everything',23,3,'Stephen Hawking','Science',160,4,1,''),
(34,'Dracula',22,3,'Bram Stoker','Horror',160,0,1,''),
(35,'Meditations',19,2,'Marcus Aurelius','Philosophy',160,3,1,''),
(36,'Frankenstein',21,2,'Mary Shelley','Gothic',160,0,1,''),
(37,'The Book Thief',23,3,'Markus Zusak','Historical Fiction',160,0,1,''),
(38,'Norwegian Wood',21,3,'Haruki Murakami','Romance',160,0,1,''),
(40,'The Little Prince',20,2,'Antoine de Saint-Exupéry','Children''s',160,0,1,''),
(42,'Animal Farm',19,2,'George Orwell','Political Satire',NULL,0,2,''),
(43,'Gone Girl',21,3,'Gillian Flynn','Thriller',161,0,1,''),
(44,'The Martian',22,3,'Andy Weir','Science Fiction',161,0,1,''),
(45,'Educated',23,3,'Tara Westover','Memoir',161,0,1,''),
(46,'The Silent Patient',21,3,'Alex Michaelides','Psychological Thriller',161,0,1,''),
(48,'Norwegian Wood',21,3,'Haruki Murakami','Fiction',161,5,1,''),
(49,'Crime and Punishment',24,4,'Fyodor Dostoevsky','Classic',161,5,1,''),
(50,'The Little Prince',18,2,'Antoine de Saint-Exupéry','Children',161,5,1,''),
(52,'Animal Farm',19,2,'George Orwell','Satire',161,5,1,''),
(53,'Gone Girl',22,3,'Gillian Flynn','Thriller',161,5,1,''),
(54,'The Martian',21,3,'Andy Weir','Sci-Fi',161,5,1,''),
(55,'Educated',23,3,'Tara Westover','Memoir',161,5,1,''),
(56,'The Silent Patient',22,3,'Alex Michaelides','Thriller',161,5,1,''),
(58,'Les Misérables',25,5,'Victor Hugo','Historical',161,5,1,''),
(59,'Thinking in Systems',20,3,'Donella H. Meadows','Nonfiction',161,5,1,''),
(60,'Rework',19,2,'Jason Fried','Business',161,5,1,''),
(61,'Persepolis',21,3,'Marjane Satrapi','Graphic Novel',161,5,1,''),
(62,'Blink',20,3,'Malcolm Gladwell','Psychology',161,5,1,''),
(63,'The Giver',18,2,'Lois Lowry','Dystopian',161,5,1,''),
(64,'The Sun Also Rises',19,2,'Ernest Hemingway','Classic',161,5,1,''),
(65,'The Handmaid’s Tale',22,3,'Margaret Atwood','Dystopian',161,5,1,''),
(66,'Kafka on the Shore',23,4,'Haruki Murakami','Fantasy',161,5,1,''),
(67,'The Kite Runner',21,3,'Khaled Hosseini','Drama',161,5,1,''),
(69,'The Silent Horizon',20,3,'Nina Carter','Sci-Fi',161,0,1,''),
(70,'Whispers in Time',22,4,'Liam Dorsey','Historical',161,1,1,''),
(71,'Crimson Waters',18,2,'Mira Stone','Fantasy',161,2,1,''),
(72,'Echo Protocol',21,3,'Tyler Knox','Techno-Thriller',NULL,3,0,'12'),
(76,'Binary Soul',20,3,'Sean Torres','Sci-Fi',NULL,4,2,''),
(77,'Legacy of Ash',23,4,'Holly Nguyen','Epic',NULL,3,2,''),
(78,'Frozen Spiral',19,2,'Daphne Lee','Thriller',NULL,0,2,''),
(79,'Solar Reign',21,3,'Marcus Wynn','Sci-Fi',NULL,1,2,''),
(80,'Harvest Key',18,2,'Isla Moreno','Mystery',NULL,2,2,''),
(81,'Gravity''s Reach',20,3,'Omar Patel','Science',162,5,1,''),
(82,'Rise of the Phoenix',22,3,'Zoe Kim','Adventure',162,4,1,''),
(83,'Blindsight',18,2,'Iris Kane','Suspense',162,0,1,''),
(84,'Golem Engine',21,3,'Vin Carter','Steampunk',162,3,1,''),
(85,'Veil of Mist',19,2,'Ayla Jensen','Fantasy',162,1,1,''),
(86,'Terminal Fall',20,3,'Grayson Wood','Thriller',162,4,1,''),
(87,'The Shadow Loop',19,2,'Ezra Bloom','Crime',162,0,1,''),
(88,'Night''s Reckoning',20,3,'Talia Ward','Drama',162,0,1,''),
(91,'The Hobbit',22,5,'J.R.R. Tolkien','Fantasy',160,0,1,''),
(101,'Tutunamayanlar',20,5,'Oğuz Atay','Fiction',160,0,1,''),
(105,'Saatleri Ayarlama Enstitüsü',20,5,'Hamdi Tanpınar','Satire',168,0,1,'');

ALTER TABLE `Bookshelves` AUTO_INCREMENT = 24;
ALTER TABLE `Shelves` AUTO_INCREMENT = 174;
ALTER TABLE `Books` AUTO_INCREMENT = 106;
