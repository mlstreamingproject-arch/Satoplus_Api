CREATE TABLE `initialtokenlogin` (
    `Id` BIGINT AUTO_INCREMENT NOT NULL,
    `Token` VARCHAR(255) NULL,
    `UserId` INT NULL,
    `Model` VARCHAR(255) NULL,
    `CreateDate` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
    `IsValidado` TINYINT(1) NULL DEFAULT 0,
    `Status` TINYINT(1) NULL DEFAULT 1,
    `UpdateDate` DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
    `DateValidate` DATE NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `UX_initialtokenlogin_token` (`Token`),
    KEY `IX_initialtokenlogin_user_id` (`UserId`),
    KEY `IX_initialtokenlogin_status` (`Status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
