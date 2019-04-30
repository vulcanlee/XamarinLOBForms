USE [LOBForm]
GO
/****** Object:  Table [dbo].[LOBLeaveAppForms]    Script Date: 2018/2/28 下午 04:02:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LOBLeaveAppForms](
	[LeaveAppFormId] [int] IDENTITY(1,1) NOT NULL,
	[FormDate] [datetime] NOT NULL,
	[Category] [nvarchar](max) NULL,
	[BeginDate] [datetime] NOT NULL,
	[CompleteDate] [datetime] NOT NULL,
	[AgentName] [nvarchar](max) NULL,
	[LeaveCause] [nvarchar](max) NULL,
	[FormsStatus] [nvarchar](max) NULL,
	[ApproveResult] [nvarchar](max) NULL,
	[Owner_MyUserId] [int] NULL,
	[Hours] [float] NOT NULL,
	[AgentId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.LOBLeaveAppForms] PRIMARY KEY CLUSTERED 
(
	[LeaveAppFormId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LOBLeaveCategories]    Script Date: 2018/2/28 下午 04:02:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LOBLeaveCategories](
	[LeaveCategoryId] [int] IDENTITY(1,1) NOT NULL,
	[SortingOrder] [int] NOT NULL,
	[LeaveCategoryName] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.LOBLeaveCategories] PRIMARY KEY CLUSTERED 
(
	[LeaveCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LOBMyUsers]    Script Date: 2018/2/28 下午 04:02:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LOBMyUsers](
	[MyUserId] [int] IDENTITY(1,1) NOT NULL,
	[DepartmentName] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[EmployeeID] [nvarchar](max) NULL,
	[Password] [nvarchar](max) NULL,
	[IsManager] [bit] NOT NULL,
	[CreatedAt] [datetimeoffset](7) NULL,
	[UpdatedAt] [datetimeoffset](7) NULL,
	[ManagerId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.LOBMyUsers] PRIMARY KEY CLUSTERED 
(
	[MyUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LOBOnCallPhones]    Script Date: 2018/2/28 下午 04:02:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LOBOnCallPhones](
	[OnCallPhoneId] [int] IDENTITY(1,1) NOT NULL,
	[SortingOrder] [int] NOT NULL,
	[Title] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.LOBOnCallPhones] PRIMARY KEY CLUSTERED 
(
	[OnCallPhoneId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LOBProjects]    Script Date: 2018/2/28 下午 04:02:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LOBProjects](
	[ProjectId] [int] IDENTITY(1,1) NOT NULL,
	[ProjectName] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.LOBProjects] PRIMARY KEY CLUSTERED 
(
	[ProjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LOBWorkingLogs]    Script Date: 2018/2/28 下午 04:02:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LOBWorkingLogs](
	[WorkingLogId] [int] IDENTITY(1,1) NOT NULL,
	[LogDate] [datetime] NOT NULL,
	[Title] [nvarchar](max) NULL,
	[Summary] [nvarchar](max) NULL,
	[Owner_MyUserId] [int] NULL,
	[Project_ProjectId] [int] NULL,
	[Hours] [float] NOT NULL,
 CONSTRAINT [PK_dbo.LOBWorkingLogs] PRIMARY KEY CLUSTERED 
(
	[WorkingLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Index [IX_Owner_MyUserId]    Script Date: 2018/2/28 下午 04:02:11 ******/
CREATE NONCLUSTERED INDEX [IX_Owner_MyUserId] ON [dbo].[LOBLeaveAppForms]
(
	[Owner_MyUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Owner_MyUserId]    Script Date: 2018/2/28 下午 04:02:11 ******/
CREATE NONCLUSTERED INDEX [IX_Owner_MyUserId] ON [dbo].[LOBWorkingLogs]
(
	[Owner_MyUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Project_ProjectId]    Script Date: 2018/2/28 下午 04:02:11 ******/
CREATE NONCLUSTERED INDEX [IX_Project_ProjectId] ON [dbo].[LOBWorkingLogs]
(
	[Project_ProjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[LOBLeaveAppForms] ADD  DEFAULT ((0)) FOR [Hours]
GO
ALTER TABLE [dbo].[LOBLeaveAppForms] ADD  DEFAULT ((0)) FOR [AgentId]
GO
ALTER TABLE [dbo].[LOBMyUsers] ADD  DEFAULT ((0)) FOR [ManagerId]
GO
ALTER TABLE [dbo].[LOBWorkingLogs] ADD  DEFAULT ((0)) FOR [Hours]
GO
ALTER TABLE [dbo].[LOBLeaveAppForms]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LOBLeaveAppForms_dbo.LOBMyUsers_Owner_MyUserId] FOREIGN KEY([Owner_MyUserId])
REFERENCES [dbo].[LOBMyUsers] ([MyUserId])
GO
ALTER TABLE [dbo].[LOBLeaveAppForms] CHECK CONSTRAINT [FK_dbo.LOBLeaveAppForms_dbo.LOBMyUsers_Owner_MyUserId]
GO
ALTER TABLE [dbo].[LOBWorkingLogs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LOBWorkingLogs_dbo.LOBMyUsers_Owner_MyUserId] FOREIGN KEY([Owner_MyUserId])
REFERENCES [dbo].[LOBMyUsers] ([MyUserId])
GO
ALTER TABLE [dbo].[LOBWorkingLogs] CHECK CONSTRAINT [FK_dbo.LOBWorkingLogs_dbo.LOBMyUsers_Owner_MyUserId]
GO
ALTER TABLE [dbo].[LOBWorkingLogs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.LOBWorkingLogs_dbo.LOBProjects_Project_ProjectId] FOREIGN KEY([Project_ProjectId])
REFERENCES [dbo].[LOBProjects] ([ProjectId])
GO
ALTER TABLE [dbo].[LOBWorkingLogs] CHECK CONSTRAINT [FK_dbo.LOBWorkingLogs_dbo.LOBProjects_Project_ProjectId]
GO
