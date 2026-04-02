USE [HMS]
GO
INSERT [dbo].[AppointmentStatus] ([Id], [Title], [Description], [CreatedById], [IsActive], [IsDelete], [CreatedDate], [ModifiedById], [ModifiedDate], [DeleteDate]) VALUES (1, N'Pending', N'Only Register', N'408c1d72-07fd-4e9a-a54c-d1ad4112f875', 1, 0, CAST(N'2026-04-01T20:40:32.3566667' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[AppointmentStatus] ([Id], [Title], [Description], [CreatedById], [IsActive], [IsDelete], [CreatedDate], [ModifiedById], [ModifiedDate], [DeleteDate]) VALUES (5, N'Confirm', N'Payment Received', N'408c1d72-07fd-4e9a-a54c-d1ad4112f875', 1, 0, CAST(N'2026-04-01T20:40:56.1666667' AS DateTime2), NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[AppointmentType] ON 
GO
INSERT [dbo].[AppointmentType] ([Id], [Code], [Name], [Description], [CompanyId], [CreatedById], [IsActive], [IsDelete], [CreatedDate], [ModifiedById], [ModifiedDate], [DeleteDate]) VALUES (1, N'S', N'Self', N'Self', 2, N'408c1d72-07fd-4e9a-a54c-d1ad4112f875', 1, 0, CAST(N'2026-04-01T20:57:14.0300000' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[AppointmentType] ([Id], [Code], [Name], [Description], [CompanyId], [CreatedById], [IsActive], [IsDelete], [CreatedDate], [ModifiedById], [ModifiedDate], [DeleteDate]) VALUES (2, N'FM', N'Family Member', N'Family Member', 2, N'408c1d72-07fd-4e9a-a54c-d1ad4112f875', 1, 0, CAST(N'2026-04-01T20:57:26.5866667' AS DateTime2), NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[AppointmentType] OFF
GO
SET IDENTITY_INSERT [dbo].[VisitType] ON 
GO
INSERT [dbo].[VisitType] ([Id], [Name], [CompanyId], [CreatedById], [IsActive], [IsDelete], [CreatedDate], [ModifiedById], [ModifiedDate], [DeleteDate]) VALUES (1, N'First Visit', 2, N'408c1d72-07fd-4e9a-a54c-d1ad4112f875', 1, 0, CAST(N'2026-04-01T20:22:07.5100000' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[VisitType] ([Id], [Name], [CompanyId], [CreatedById], [IsActive], [IsDelete], [CreatedDate], [ModifiedById], [ModifiedDate], [DeleteDate]) VALUES (2, N'Follow-up', 2, N'408c1d72-07fd-4e9a-a54c-d1ad4112f875', 1, 0, CAST(N'2026-04-01T20:22:16.3533333' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[VisitType] ([Id], [Name], [CompanyId], [CreatedById], [IsActive], [IsDelete], [CreatedDate], [ModifiedById], [ModifiedDate], [DeleteDate]) VALUES (3, N'Emergency', 2, N'408c1d72-07fd-4e9a-a54c-d1ad4112f875', 1, 0, CAST(N'2026-04-01T20:22:24.3366667' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[VisitType] ([Id], [Name], [CompanyId], [CreatedById], [IsActive], [IsDelete], [CreatedDate], [ModifiedById], [ModifiedDate], [DeleteDate]) VALUES (4, N'Tele-Consultation', 2, N'408c1d72-07fd-4e9a-a54c-d1ad4112f875', 1, 0, CAST(N'2026-04-01T20:22:36.5300000' AS DateTime2), NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[VisitType] OFF
GO

SET IDENTITY_INSERT [dbo].[PriorityLevel] ON 
GO
INSERT [dbo].[PriorityLevel] ([Id], [Name], [CompanyId], [CreatedById], [IsActive], [IsDelete], [CreatedDate], [ModifiedById], [ModifiedDate], [DeleteDate]) VALUES (1, N'Normal', 2, N'408c1d72-07fd-4e9a-a54c-d1ad4112f875', 1, 0, CAST(N'2026-04-01T20:37:34.0300000' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[PriorityLevel] ([Id], [Name], [CompanyId], [CreatedById], [IsActive], [IsDelete], [CreatedDate], [ModifiedById], [ModifiedDate], [DeleteDate]) VALUES (2, N'Urgent', 2, N'408c1d72-07fd-4e9a-a54c-d1ad4112f875', 1, 0, CAST(N'2026-04-01T20:37:40.9666667' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[PriorityLevel] ([Id], [Name], [CompanyId], [CreatedById], [IsActive], [IsDelete], [CreatedDate], [ModifiedById], [ModifiedDate], [DeleteDate]) VALUES (3, N'Emergency', 2, N'408c1d72-07fd-4e9a-a54c-d1ad4112f875', 1, 0, CAST(N'2026-04-01T20:37:44.8900000' AS DateTime2), NULL, NULL, NULL)
GO
INSERT [dbo].[PriorityLevel] ([Id], [Name], [CompanyId], [CreatedById], [IsActive], [IsDelete], [CreatedDate], [ModifiedById], [ModifiedDate], [DeleteDate]) VALUES (4, N'Critical', 2, N'408c1d72-07fd-4e9a-a54c-d1ad4112f875', 1, 0, CAST(N'2026-04-01T20:37:49.8400000' AS DateTime2), NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[PriorityLevel] OFF
GO