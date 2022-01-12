using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Evote_Service.Migrations
{
    public partial class Add1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AdminLoginLogs",
                columns: table => new
                {
                    AdminLoginLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Cmuaccount = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LoginTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ClientIP = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminLoginLogs", x => x.AdminLoginLogId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApplicationEntitys",
                columns: table => new
                {
                    ApplicationEntityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ApplicationName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LineAuth = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CMUAuth = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ServerProductionIP = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationEntitys", x => x.ApplicationEntityId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EventStatus",
                columns: table => new
                {
                    EventStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EventStatusName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStatus", x => x.EventStatusId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RefUserStages",
                columns: table => new
                {
                    RefUserStageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserStageName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefUserStages", x => x.RefUserStageID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserAdminEntitys",
                columns: table => new
                {
                    UserAdminEntityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FullName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cmuaccount = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tel = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SuperAdmin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Organization_Code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrganizationFullNameTha = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SMSOTP = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SMSOTPRef = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SMSExpire = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Access_token = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Refresh_token = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAdminEntitys", x => x.UserAdminEntityId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserEntitys",
                columns: table => new
                {
                    UserEntityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserCode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Organization_Code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Organization_Name_TH = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FullName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PersonalID = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tel = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserStage = table.Column<int>(type: "int", nullable: false),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    LineId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    SMSOTP = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SMSOTPRef = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SMSExpire = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    EmailOTP = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailOTPRef = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsConfirmEmail = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ConfirmEmailTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsConfirmTel = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ConfirmTelTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsConfirmPersonalID = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ConfirmPersonalIDTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    fileNamePersonalID = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fullPathPersonalID = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dbPathPersonalID = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsConfirmKYC = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ConfirmKYCTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    fileNameKYC = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fullPathKYC = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dbPathKYC = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fileNameFace = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fullPathFace = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dbPathFace = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    faceData = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdminApproved = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdminApprovedIP = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdminNotApproved = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdminNotApprovedIP = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CommetNotApproved = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ApprovedTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    NotApprovedTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Access_token = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Refresh_token = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Expires_in = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEntitys", x => x.UserEntityId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EventVoteEntitys",
                columns: table => new
                {
                    EventVoteEntityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EventStatusId = table.Column<int>(type: "int", nullable: false),
                    ApplicationEntityId = table.Column<int>(type: "int", nullable: false),
                    EventTypeId = table.Column<int>(type: "int", nullable: false),
                    EventInformation = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecretKey = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecurityAlgorithm = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EventTitle = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EventDetail = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateUser = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PresidentEmail = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PresidentUpdate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdateUser = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Organization_Code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrganizationFullNameTha = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EventCreate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EventUpdate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EventRegisterStart = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EventRegisterEnd = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EventVotingStart = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EventVotingEnd = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsEnd = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDev = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventVoteEntitys", x => x.EventVoteEntityId);
                    table.ForeignKey(
                        name: "FK_EventVoteEntitys_ApplicationEntitys_ApplicationEntityId",
                        column: x => x.ApplicationEntityId,
                        principalTable: "ApplicationEntitys",
                        principalColumn: "ApplicationEntityId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VoterEntitys",
                columns: table => new
                {
                    VoterEntityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EventVoteEntityId = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FullName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    Organization_Code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateUser = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VoterCreate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    SMSOTP = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SMSOTPRef = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SMSExpire = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoterEntitys", x => x.VoterEntityId);
                    table.ForeignKey(
                        name: "FK_VoterEntitys_EventVoteEntitys_EventVoteEntityId",
                        column: x => x.EventVoteEntityId,
                        principalTable: "EventVoteEntitys",
                        principalColumn: "EventVoteEntityId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "voteRoundEntities",
                columns: table => new
                {
                    VoteRoundEntityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EventVoteEntityId = table.Column<int>(type: "int", nullable: false),
                    RoundNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_voteRoundEntities", x => x.VoteRoundEntityId);
                    table.ForeignKey(
                        name: "FK_voteRoundEntities_EventVoteEntitys_EventVoteEntityId",
                        column: x => x.EventVoteEntityId,
                        principalTable: "EventVoteEntitys",
                        principalColumn: "EventVoteEntityId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "confirmVoters",
                columns: table => new
                {
                    ConfirmVoterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VoteRoundEntityId = table.Column<int>(type: "int", nullable: false),
                    EventVoteEntityId = table.Column<int>(type: "int", nullable: false),
                    RoundNumber = table.Column<int>(type: "int", nullable: false),
                    email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_confirmVoters", x => x.ConfirmVoterId);
                    table.ForeignKey(
                        name: "FK_confirmVoters_voteRoundEntities_VoteRoundEntityId",
                        column: x => x.VoteRoundEntityId,
                        principalTable: "voteRoundEntities",
                        principalColumn: "VoteRoundEntityId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "voteEntities",
                columns: table => new
                {
                    VoteEntityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ApplicationEntityId = table.Column<int>(type: "int", nullable: false),
                    EventVoteEntityId = table.Column<int>(type: "int", nullable: false),
                    VoteRoundEntityId = table.Column<int>(type: "int", nullable: false),
                    RoundNumber = table.Column<int>(type: "int", nullable: false),
                    VoteData = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_voteEntities", x => x.VoteEntityId);
                    table.ForeignKey(
                        name: "FK_voteEntities_voteRoundEntities_VoteRoundEntityId",
                        column: x => x.VoteRoundEntityId,
                        principalTable: "voteRoundEntities",
                        principalColumn: "VoteRoundEntityId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_confirmVoters_VoteRoundEntityId",
                table: "confirmVoters",
                column: "VoteRoundEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EventVoteEntitys_ApplicationEntityId",
                table: "EventVoteEntitys",
                column: "ApplicationEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_voteEntities_VoteRoundEntityId",
                table: "voteEntities",
                column: "VoteRoundEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_VoterEntitys_EventVoteEntityId",
                table: "VoterEntitys",
                column: "EventVoteEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_voteRoundEntities_EventVoteEntityId",
                table: "voteRoundEntities",
                column: "EventVoteEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminLoginLogs");

            migrationBuilder.DropTable(
                name: "confirmVoters");

            migrationBuilder.DropTable(
                name: "EventStatus");

            migrationBuilder.DropTable(
                name: "RefUserStages");

            migrationBuilder.DropTable(
                name: "UserAdminEntitys");

            migrationBuilder.DropTable(
                name: "UserEntitys");

            migrationBuilder.DropTable(
                name: "voteEntities");

            migrationBuilder.DropTable(
                name: "VoterEntitys");

            migrationBuilder.DropTable(
                name: "voteRoundEntities");

            migrationBuilder.DropTable(
                name: "EventVoteEntitys");

            migrationBuilder.DropTable(
                name: "ApplicationEntitys");
        }
    }
}
