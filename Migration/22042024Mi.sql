CREATE TABLE `tb_users` (
	`id` INT(11) NOT NULL AUTO_INCREMENT,
	`name` VARCHAR(50) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	`password` VARCHAR(255) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	`office` VARCHAR(50) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	`refresh_token` VARCHAR(500) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	`refresh_token_expiry_time` DATETIME NULL DEFAULT NULL,
	PRIMARY KEY (`id`) USING BTREE,
	UNIQUE INDEX `name` (`name`) USING BTREE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
AUTO_INCREMENT=34
;
