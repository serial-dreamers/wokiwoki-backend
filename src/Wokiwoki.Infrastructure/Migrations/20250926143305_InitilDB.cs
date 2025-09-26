using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Wokiwoki.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitilDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "aspnetroles",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedname = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrencystamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetroles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "aspnetusers",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedusername = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedemail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    emailconfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    passwordhash = table.Column<string>(type: "text", nullable: true),
                    securitystamp = table.Column<string>(type: "text", nullable: true),
                    concurrencystamp = table.Column<string>(type: "text", nullable: true),
                    phonenumber = table.Column<string>(type: "text", nullable: true),
                    phonenumberconfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    twofactorenabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockoutend = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockoutenabled = table.Column<bool>(type: "boolean", nullable: false),
                    accessfailedcount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetusers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "auditlog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    entityname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    entityid = table.Column<Guid>(type: "uuid", nullable: true),
                    originalvalue = table.Column<string>(type: "text", nullable: true),
                    newvalue = table.Column<string>(type: "text", nullable: true),
                    performedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<Guid>(type: "uuid", nullable: true),
                    lastmodified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastmodifiedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auditlog", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    iconurl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    imageurl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    isactive = table.Column<bool>(type: "boolean", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<Guid>(type: "uuid", nullable: true),
                    lastmodified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastmodifiedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "organization",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    logourl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    contactemail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    contactphone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    street = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ward = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    district = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    province = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    isactive = table.Column<bool>(type: "boolean", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<Guid>(type: "uuid", nullable: true),
                    lastmodified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastmodifiedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "refresh_token",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    expiresat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revoked = table.Column<bool>(type: "boolean", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<Guid>(type: "uuid", nullable: true),
                    lastmodified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastmodifiedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_token", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tag",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    iconurl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    isactive = table.Column<bool>(type: "boolean", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<Guid>(type: "uuid", nullable: true),
                    lastmodified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastmodifiedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tag", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "workshop_type",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    iconurl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    isactive = table.Column<bool>(type: "boolean", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<Guid>(type: "uuid", nullable: true),
                    lastmodified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastmodifiedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workshop_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "aspnetroleclaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roleid = table.Column<string>(type: "text", nullable: false),
                    claimtype = table.Column<string>(type: "text", nullable: true),
                    claimvalue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetroleclaims", x => x.id);
                    table.ForeignKey(
                        name: "fk_aspnetroleclaims_aspnetroles_roleid",
                        column: x => x.roleid,
                        principalTable: "aspnetroles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnetuserclaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<string>(type: "text", nullable: false),
                    claimtype = table.Column<string>(type: "text", nullable: true),
                    claimvalue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetuserclaims", x => x.id);
                    table.ForeignKey(
                        name: "fk_aspnetuserclaims_aspnetusers_userid",
                        column: x => x.userid,
                        principalTable: "aspnetusers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnetuserlogins",
                columns: table => new
                {
                    loginprovider = table.Column<string>(type: "text", nullable: false),
                    providerkey = table.Column<string>(type: "text", nullable: false),
                    providerdisplayname = table.Column<string>(type: "text", nullable: true),
                    userid = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetuserlogins", x => new { x.loginprovider, x.providerkey });
                    table.ForeignKey(
                        name: "fk_aspnetuserlogins_aspnetusers_userid",
                        column: x => x.userid,
                        principalTable: "aspnetusers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnetuserroles",
                columns: table => new
                {
                    userid = table.Column<string>(type: "text", nullable: false),
                    roleid = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetuserroles", x => new { x.userid, x.roleid });
                    table.ForeignKey(
                        name: "fk_aspnetuserroles_aspnetroles_roleid",
                        column: x => x.roleid,
                        principalTable: "aspnetroles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_aspnetuserroles_aspnetusers_userid",
                        column: x => x.userid,
                        principalTable: "aspnetusers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnetusertokens",
                columns: table => new
                {
                    userid = table.Column<string>(type: "text", nullable: false),
                    loginprovider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetusertokens", x => new { x.userid, x.loginprovider, x.name });
                    table.ForeignKey(
                        name: "fk_aspnetusertokens_aspnetusers_userid",
                        column: x => x.userid,
                        principalTable: "aspnetusers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "organization_member",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    joinedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    isactive = table.Column<bool>(type: "boolean", nullable: false),
                    organizationid = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<Guid>(type: "uuid", nullable: true),
                    lastmodified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastmodifiedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization_member", x => x.id);
                    table.ForeignKey(
                        name: "fk_organization_member_organization_organizationid",
                        column: x => x.organizationid,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "category_tag",
                columns: table => new
                {
                    CategoriesId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category_tag", x => new { x.CategoriesId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_category_tag_category_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_category_tag_tag_TagsId",
                        column: x => x.TagsId,
                        principalTable: "tag",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_tag_preference",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    tagid = table.Column<Guid>(type: "uuid", nullable: false),
                    categoryid = table.Column<Guid>(type: "uuid", nullable: false),
                    applicationuserid = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<Guid>(type: "uuid", nullable: true),
                    lastmodified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastmodifiedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_tag_preference", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_tag_preference_aspnetusers_applicationuserid",
                        column: x => x.applicationuserid,
                        principalTable: "aspnetusers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_user_tag_preference_category_categoryid",
                        column: x => x.categoryid,
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_tag_preference_tag_tagid",
                        column: x => x.tagid,
                        principalTable: "tag",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "workshop",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    shortdescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    imageurl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    starttime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    endtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    organizationid = table.Column<Guid>(type: "uuid", nullable: false),
                    categoryid = table.Column<Guid>(type: "uuid", nullable: false),
                    isactive = table.Column<bool>(type: "boolean", nullable: false),
                    workshoptypeid = table.Column<Guid>(type: "uuid", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<Guid>(type: "uuid", nullable: true),
                    lastmodified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastmodifiedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workshop", x => x.id);
                    table.ForeignKey(
                        name: "fk_workshop_category_categoryid",
                        column: x => x.categoryid,
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_workshop_organization_organizationid",
                        column: x => x.organizationid,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_workshop_workshop_type_workshoptypeid",
                        column: x => x.workshoptypeid,
                        principalTable: "workshop_type",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "booking",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    workshopid = table.Column<Guid>(type: "uuid", nullable: false),
                    totalprice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    isactive = table.Column<bool>(type: "boolean", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<Guid>(type: "uuid", nullable: true),
                    lastmodified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastmodifiedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_booking", x => x.id);
                    table.ForeignKey(
                        name: "fk_booking_workshop_workshopid",
                        column: x => x.workshopid,
                        principalTable: "workshop",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_workshop_like",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    workshopid = table.Column<Guid>(type: "uuid", nullable: false),
                    likedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<Guid>(type: "uuid", nullable: true),
                    lastmodified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastmodifiedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_workshop_like", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_workshop_like_workshop_workshopid",
                        column: x => x.workshopid,
                        principalTable: "workshop",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workshop_media",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    imageurl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    mediatype = table.Column<int>(type: "integer", nullable: false),
                    uploadedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    uploadedbyuserid = table.Column<Guid>(type: "uuid", nullable: false),
                    workshopid = table.Column<Guid>(type: "uuid", nullable: false),
                    isactive = table.Column<bool>(type: "boolean", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<Guid>(type: "uuid", nullable: true),
                    lastmodified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastmodifiedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workshop_media", x => x.id);
                    table.ForeignKey(
                        name: "fk_workshop_media_workshop_workshopid",
                        column: x => x.workshopid,
                        principalTable: "workshop",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workshop_session",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    starttime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    endtime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    workshopid = table.Column<Guid>(type: "uuid", nullable: false),
                    isactive = table.Column<bool>(type: "boolean", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<Guid>(type: "uuid", nullable: true),
                    lastmodified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastmodifiedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workshop_session", x => x.id);
                    table.ForeignKey(
                        name: "fk_workshop_session_workshop_workshopid",
                        column: x => x.workshopid,
                        principalTable: "workshop",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workshop_tag",
                columns: table => new
                {
                    TagsId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkshopsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workshop_tag", x => new { x.TagsId, x.WorkshopsId });
                    table.ForeignKey(
                        name: "FK_workshop_tag_tag_TagsId",
                        column: x => x.TagsId,
                        principalTable: "tag",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_workshop_tag_workshop_WorkshopsId",
                        column: x => x.WorkshopsId,
                        principalTable: "workshop",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workshop_hero_media",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    herotype = table.Column<int>(type: "integer", nullable: false),
                    galleryid = table.Column<Guid>(type: "uuid", nullable: true),
                    workshopid = table.Column<Guid>(type: "uuid", nullable: false),
                    isactive = table.Column<bool>(type: "boolean", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<Guid>(type: "uuid", nullable: true),
                    lastmodified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastmodifiedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workshop_hero_media", x => x.id);
                    table.ForeignKey(
                        name: "fk_workshop_hero_media_workshop_media_galleryid",
                        column: x => x.galleryid,
                        principalTable: "workshop_media",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_workshop_hero_media_workshop_workshopid",
                        column: x => x.workshopid,
                        principalTable: "workshop",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workshop_ticket_type",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    sold = table.Column<int>(type: "integer", nullable: false),
                    salestart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    saleend = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    workshopsessionid = table.Column<Guid>(type: "uuid", nullable: false),
                    isactive = table.Column<bool>(type: "boolean", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<Guid>(type: "uuid", nullable: true),
                    lastmodified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastmodifiedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workshop_ticket_type", x => x.id);
                    table.CheckConstraint("CK_WorkshopTicketType_Price", "price >= 0");
                    table.ForeignKey(
                        name: "fk_workshop_ticket_type_workshop_session_workshopsessionid",
                        column: x => x.workshopsessionid,
                        principalTable: "workshop_session",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ticket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    bookingid = table.Column<Guid>(type: "uuid", nullable: false),
                    tickettypeid = table.Column<Guid>(type: "uuid", nullable: false),
                    qrcodeimage = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    isactive = table.Column<bool>(type: "boolean", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<Guid>(type: "uuid", nullable: true),
                    lastmodified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastmodifiedby = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket", x => x.id);
                    table.ForeignKey(
                        name: "fk_ticket_booking_bookingid",
                        column: x => x.bookingid,
                        principalTable: "booking",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ticket_workshop_ticket_type_tickettypeid",
                        column: x => x.tickettypeid,
                        principalTable: "workshop_ticket_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_aspnetroleclaims_roleid",
                table: "aspnetroleclaims",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "rolenameindex",
                table: "aspnetroles",
                column: "normalizedname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_aspnetuserclaims_userid",
                table: "aspnetuserclaims",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_aspnetuserlogins_userid",
                table: "aspnetuserlogins",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_aspnetuserroles_roleid",
                table: "aspnetuserroles",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "emailindex",
                table: "aspnetusers",
                column: "normalizedemail");

            migrationBuilder.CreateIndex(
                name: "usernameindex",
                table: "aspnetusers",
                column: "normalizedusername",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_UserId",
                table: "booking",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_WorkshopId",
                table: "booking",
                column: "workshopid");

            migrationBuilder.CreateIndex(
                name: "IX_category_tag_TagsId",
                table: "category_tag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "ix_organization_member_organizationid",
                table: "organization_member",
                column: "organizationid");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_Token",
                table: "refresh_token",
                column: "token");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId",
                table: "refresh_token",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_bookingid",
                table: "ticket",
                column: "bookingid");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_tickettypeid",
                table: "ticket",
                column: "tickettypeid");

            migrationBuilder.CreateIndex(
                name: "ix_user_tag_preference_applicationuserid",
                table: "user_tag_preference",
                column: "applicationuserid");

            migrationBuilder.CreateIndex(
                name: "ix_user_tag_preference_categoryid",
                table: "user_tag_preference",
                column: "categoryid");

            migrationBuilder.CreateIndex(
                name: "ix_user_tag_preference_tagid",
                table: "user_tag_preference",
                column: "tagid");

            migrationBuilder.CreateIndex(
                name: "IX_user_tag_preference_userid_tagid_categoryid",
                table: "user_tag_preference",
                columns: new[] { "userid", "tagid", "categoryid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_workshop_like_userid_workshopid",
                table: "user_workshop_like",
                columns: new[] { "userid", "workshopid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_workshop_like_workshopid",
                table: "user_workshop_like",
                column: "workshopid");

            migrationBuilder.CreateIndex(
                name: "IX_Workshop_CategoryId",
                table: "workshop",
                column: "categoryid");

            migrationBuilder.CreateIndex(
                name: "IX_Workshop_OrganizationId",
                table: "workshop",
                column: "organizationid");

            migrationBuilder.CreateIndex(
                name: "IX_Workshop_StartTime",
                table: "workshop",
                column: "starttime");

            migrationBuilder.CreateIndex(
                name: "ix_workshop_workshoptypeid",
                table: "workshop",
                column: "workshoptypeid");

            migrationBuilder.CreateIndex(
                name: "ix_workshop_hero_media_galleryid",
                table: "workshop_hero_media",
                column: "galleryid");

            migrationBuilder.CreateIndex(
                name: "ix_workshop_hero_media_workshopid",
                table: "workshop_hero_media",
                column: "workshopid");

            migrationBuilder.CreateIndex(
                name: "ix_workshop_media_workshopid",
                table: "workshop_media",
                column: "workshopid");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopSession_WorkshopId",
                table: "workshop_session",
                column: "workshopid");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_tag_WorkshopsId",
                table: "workshop_tag",
                column: "WorkshopsId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopTicketType_WorkshopSessionId",
                table: "workshop_ticket_type",
                column: "workshopsessionid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aspnetroleclaims");

            migrationBuilder.DropTable(
                name: "aspnetuserclaims");

            migrationBuilder.DropTable(
                name: "aspnetuserlogins");

            migrationBuilder.DropTable(
                name: "aspnetuserroles");

            migrationBuilder.DropTable(
                name: "aspnetusertokens");

            migrationBuilder.DropTable(
                name: "auditlog");

            migrationBuilder.DropTable(
                name: "category_tag");

            migrationBuilder.DropTable(
                name: "organization_member");

            migrationBuilder.DropTable(
                name: "refresh_token");

            migrationBuilder.DropTable(
                name: "ticket");

            migrationBuilder.DropTable(
                name: "user_tag_preference");

            migrationBuilder.DropTable(
                name: "user_workshop_like");

            migrationBuilder.DropTable(
                name: "workshop_hero_media");

            migrationBuilder.DropTable(
                name: "workshop_tag");

            migrationBuilder.DropTable(
                name: "aspnetroles");

            migrationBuilder.DropTable(
                name: "booking");

            migrationBuilder.DropTable(
                name: "workshop_ticket_type");

            migrationBuilder.DropTable(
                name: "aspnetusers");

            migrationBuilder.DropTable(
                name: "workshop_media");

            migrationBuilder.DropTable(
                name: "tag");

            migrationBuilder.DropTable(
                name: "workshop_session");

            migrationBuilder.DropTable(
                name: "workshop");

            migrationBuilder.DropTable(
                name: "category");

            migrationBuilder.DropTable(
                name: "organization");

            migrationBuilder.DropTable(
                name: "workshop_type");
        }
    }
}
