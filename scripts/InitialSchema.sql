﻿CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE "Checkouts" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "CheckoutDate" timestamp without time zone NOT NULL,
    "DueDate" timestamp without time zone NOT NULL,
    "ReturnDate" timestamp without time zone NOT NULL,
    "CheckoutStatus" integer NOT NULL,
    "ApprovalStatus" integer NOT NULL,
    CONSTRAINT "PK_Checkouts" PRIMARY KEY ("Id")
);

CREATE TABLE "ItemGroups" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "Moniker" text NULL,
    "Make" text NULL,
    "Model" text NULL,
    "Description" text NULL,
    "Category" text NULL,
    "Department" text NULL,
    CONSTRAINT "PK_ItemGroups" PRIMARY KEY ("Id")
);

CREATE TABLE "Items" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "SerialNumber" text NULL,
    "Availability" integer NOT NULL,
    "OffsiteEligable" boolean NOT NULL,
    "DateAcquired" timestamp without time zone NOT NULL,
    "LastInspected" timestamp without time zone NOT NULL,
    "Condition" integer NOT NULL,
    "Notes" text NULL,
    "CheckoutId" integer NULL,
    "ItemGroupId" integer NULL,
    CONSTRAINT "PK_Items" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Items_Checkouts_CheckoutId" FOREIGN KEY ("CheckoutId") REFERENCES "Checkouts" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Items_ItemGroups_ItemGroupId" FOREIGN KEY ("ItemGroupId") REFERENCES "ItemGroups" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_Items_CheckoutId" ON "Items" ("CheckoutId");

CREATE INDEX "IX_Items_ItemGroupId" ON "Items" ("ItemGroupId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200704212055_InitialDbMigration', '3.1.6');
