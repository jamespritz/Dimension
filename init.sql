
/*


	drop table dxblog.postrating
	drop table dxblog.referral
	drop table dxblog.posttag
	drop table dxblog.postcomment
	drop table dxblog.postcategory
	drop table dxblog.category
	drop table dxblog.post
	drop table dxblog.blog

	drop table dxxl8.resourcesubscriptionlocale
	drop table dxxl8.resourcesubscriptionkeyword
	drop table dxxl8.publicationsubscription 
	drop table dxxl8.resourcesubscription
	drop table dxxl8.keyword
	drop table dxxl8.publicationlocale
	drop table dxxl8.publication
	drop table dxxl8.subscriberaccount
	drop table dxxl8.subscriber

	drop table dxsecurity.accountsession
	drop table dxsecurity.credential
	drop table dxsecurity.account
	
	drop table dxdimension.variable
	drop table dxdimension.expressioncontext
	drop table dxdimension.expressiondefinitioncontextdefinition
	drop table dxdimension.contextdefinition
	
	drop table dxdimension.audit
	drop table dxdimension.expression
	drop table dxdimension.variabledefinition
	drop table dxdimension.expressiondefinitionobjecttype
	drop table dxdimension.expressiondefinition
	drop table dxdimension.xtype

	drop table dxxl8.xlate

	drop table dxdiag.log
	drop table dxdiag.session
	
	drop table dxintegration.objectrelationship
	drop table dxintegration.object
	drop table dxdimension.objecttyperelationship
	drop table dxdimension.objecttype

	drop table dxxl8.resource	

	
	select * from dxxl8.resource
*/

create schema dxxl8 authorization dbo
go
create schema dxtest authorization dbo
go
create schema dxintegration authorization dbo
go
create schema dxsecurity authorization dbo
go
create schema dxdiag authorization dbo
go
create schema dxdimension authorization dbo
go
create schema dxblog authorization dbo
go


/* ================================================
	XL8

	this is the subscriber's version, dumbed down to just what is needed
*/

Create Table dxxl8.resource (
	ID int not null identity(1,1) constraint pk_xl8_resource primary key
	, UID uniqueidentifier not null constraint def_xl8_resource_UID default (newid())
)

Create Table dxxl8.xlate (
	ID int not null identity(1,1) constraint pk_xl8_xlate primary key
	, resourceID int not null constraint fk_xl8_xlate_resource foreign key references dxxl8.resource (ID)
	, LCID int not null
)
create unique index uq_xl8_xlate_resourceid_lcid on dxxl8.xlate (resourceid, lcid)




/* ================================================
	Diagnostics
*/
create table dxdiag.session (
	id int not null identity(1,1) constraint pk_diag_session_id primary key
	, UID uniqueidentifier not null
	, createDT datetimeoffset not null
	, expireDT datetimeoffset null
	, origin nvarchar(255) not null
)


create table dxdiag.log (
	ID int not null identity(1,1) constraint pk_diag_log_id primary key
	, logTypeID int not null
	, entry nvarchar(max) not null
	, severity int not null
	, createDT datetimeoffset not null constraint def_diag_log_createdt default getutcdate()
	, referenceKey int null
	, sessionID int null constraint fk_diag_log_session foreign key references dxdiag.session (ID)
)

/* ================================================
	Integration
*/

create table dxdimension.xtype
	(
	ID int not null identity(1,1) constraint pk_integration_xtype primary key
	,literal_resourceID int not null 
		--constraint fk_integration_xtype_xl8_resource_literal foreign key references dxxl8.resource (ID)
	,desc_resourceID int null 
		--constraint fk_integration_xtype_xl8_resource_desc foreign key references dxxl8.resource (ID)
	,rv rowversion
	)

create table dxdimension.objecttype
	(
	ID int not null identity(1,1) constraint pk_integration_objecttype primary key
	,literal_resourceID int not null 
		--constraint fk_integration_objecttype_xl8_resource_literal foreign key references dxxl8.resource (ID)
	,desc_resourceID int null 
		--constraint fk_integration_objecttype_xl8_resource_desc foreign key references dxxl8.resource (ID)
	,rv rowversion
	)

create table dxdimension.objecttyperelationship
	(
	ID int not null  identity(1,1) constraint pk_integration_objecttyperela primary key
	,parentID int not null constraint fk_objtyperela_objecttype_parent foreign key references dxdimension.objecttype(id)
	,childID int not null constraint fk_objtyperela_objecttype_child foreign key references dxdimension.objecttype(id)
	,multiParent tinyint not null constraint def_objecttyperela_multiparent default 0
	,rv rowversion
	)
alter table dxdimension.objecttyperelationship add constraint uq_objecttyperela_parent_child unique (parentID, childID)

create table dxintegration.object 
	(
	ID int not null identity(1,1) constraint pk_integration_object primary key
	,surrogateKey nvarchar(255) not null
	,objectTypeID int not null --constraint fk_object_objecttype foreign key references dxdimension.objecttype (ID)
	,displayName nvarchar(255) not null
	,rv rowversion
	)
alter table dxintegration.object add constraint uq_object_key_type unique (surrogateKey, objectTypeID)

create table dxintegration.objectrelationship
	(
	ID int not null identity(1,1) constraint pk_integration_objectrelationship primary key
	,parentID int not null constraint fk_objectrela_object_parent foreign key references dxintegration.object (ID)
	,childID int not null constraint fk_objectrela_object_child foreign key references dxintegration.object (ID)
	,rv rowversion
	)
alter table dxintegration.objectrelationship add constraint uq_objectrela_parent_child unique (parentID, childID)




create table dxdimension.expressiondefinition
	(
	ID int not null identity(1,1) constraint pk_dimension_expressiondefinition primary key
	,UID uniqueidentifier not null constraint def_expdef_uid default newid()
	,literal_resourceID int not null 
		--constraint fk_dimension_expressiondefinition_xl8_resource_literal foreign key references dxxl8.resource (ID)
	,desc_resourceID int null 
		--constraint fk_dimension_expressiondefinition_xl8_resource_desc foreign key references dxxl8.resource (ID)
	,rv rowversion
	)
	

	
create table dxdimension.expressiondefinitionobjecttype
	(
	ID int not null identity(1,1) constraint pk_dimension_expressiondefinitionobjecttype primary key
	,expressionDefinitionID int not null constraint fk_expdefobjtype_expdef foreign key references dxdimension.expressiondefinition (ID)
	,objectTypeID int not null 
		--constraint fk_expdefobjtype_objtype foreign key references dxdimension.objecttype (ID)
	,allowInherit tinyint not null constraint def_dimension_expdefobjtype_allowinherit default(1)
	,rv rowversion
	)
alter table dxdimension.expressiondefinitionobjecttype
	add constraint uq_expdefobjtype_expdefid_objtypeid unique (expressiondefinitionid, objecttypeid)
	
create table dxdimension.contextdefinition
	(
	ID int not null identity(1,1) constraint pk_dimension_contextdefinition primary key
	,literal_resourceID int not null 
		--constraint fk_dimension_contextdefinition_xl8_resource_literal foreign key references dxxl8.resource (ID)
	,desc_resourceID int null 
		--constraint fk_dimension_contextdefinition_xl8_resource_desc foreign key references dxxl8.resource (ID)
	,xtypeID int not null constraint fk_ctxdef_xtype foreign key references dxdimension.xtype(ID)
	,rv rowversion
	)

	


create table dxdimension.expressiondefinitioncontextdefinition
	(
	ID int not null identity(1,1) constraint pk_dimension_expdefctxdef primary key
	,expressionDefinitionID int not null constraint fk_expdefctxdef_expdef foreign key references
		dxdimension.expressiondefinition (ID)
	,contextDefinitionID int not null constraint fk_expdefctxdef_ctxdef foreign key references
		dxdimension.contextdefinition (ID)
	,allowGeneric tinyint not null constraint def_expdefctxdef_allowgeneric default 0
	,rv rowversion
	)
alter table dxdimension.expressiondefinitioncontextdefinition
	add constraint uq_expdefctxdef_expdefid_ctxdefid unique (expressiondefinitionid, contextdefinitionid)
	


	
create table dxdimension.variabledefinition
	(
	ID int not null identity(1,1) constraint pk_dimension_variabledefinition primary key
	,variableKey nvarchar(50) not null
	,expressionDefinitionID int not null constraint fk_vardef_expdef foreign key references dxdimension.expressiondefinition (ID)
	,literal_resourceID int not null constraint fk_dimension_variabledefinition_xl8_resource_literal 
		foreign key references dxxl8.resource (ID)
	,desc_resourceID int null 
		--constraint fk_dimension_variabledefinition_xl8_resource_desc foreign key references dxxl8.resource (ID)
	,xtypeID int not null constraint fk_vardef_xtype foreign key references dxdimension.xtype (ID)
	,rv rowversion
	) 
alter table dxdimension.variabledefinition add constraint
	uq_vardef_expdefid_key unique (variableKey, expressiondefinitionid)

create table dxdimension.expression
	(
	ID int not null identity(1,1) constraint pk_dimension_expression primary key
	,objectID int not null --constraint fk_exp_obj foreign key references dxintegration.object (ID)
	,derivedFrom int null 
	,rv rowversion
	)
alter table dxdimension.expression 
	add constraint fk_exp_exp foreign key (derivedFrom) references dxdimension.expression (ID)

	
create table dxdimension.expressioncontext
	(
	ID int not null identity(1,1) constraint pk_dimension_expressioncontext primary key
	,expressionID int not null constraint fk_expctx_exp foreign key references dxdimension.expression (ID)
	,contextdefinitionID int not null constraint fk_expctx_ctxdef foreign key references dxdimension.contextdefinition (ID)
	,value nvarchar(max)
	,rv rowversion
	)

create table dxdimension.audit
	(
	ID int not null identity(1,1) constraint pk_dimension_audit primary key
	,expressionID int not null constraint fk_audit_expression foreign key references dxdimension.expression (ID)
	,activeDT datetime not null constraint def_audit_activedt default getutcdate()
	,expireDT datetime null
	,rv rowversion
	)
	
create table dxdimension.variable
	(
	ID int not null identity(1,1) constraint pk_dimension_variable primary key
	,expressionDefinitionID int not null constraint fk_var_expdef foreign key references 
		dxdimension.expressiondefinition (ID)
	,auditID int not null constraint fk_var_audit foreign key references dxdimension.audit (ID)
	,value nvarchar(max)
	,rv rowversion
	)


create table dxsecurity.account (
	ID int not null identity(1,1) constraint pk_security_account_id primary key
	,UID uniqueidentifier not null
	,displayName nchar(50) not null
	, createDT datetimeoffset not null constraint def_security_account_createdt default getutcdate()
	, enabled tinyint not null constraint def_security_account_enabled default 1
	,rv rowversion
)

create table dxsecurity.credential (
	ID int not null identity(1,1) constraint pk_security_credential_id primary key
	, accountID int not null constraint fk_security_credential_account
		foreign key references dxsecurity.account ( ID )
	, token nvarchar(255) null
	, credential nvarchar(1000) null
	, salt nvarchar(255) null
	, provider nchar(100) not null
	, createDT datetimeoffset not null constraint def_security_credential_createdt default getutcdate()
	, expireDT datetimeoffset null
	, renewDT datetimeoffset null
	, attempts int not null constraint def_security_credential_attempts default 0
	, enabled tinyint not null constraint def_security_credential_enabled default 1
	,rv rowversion
)
alter table dxsecurity.credential add constraint uq_security_credential_token unique (token)


create table dxsecurity.accountsession (
	id int not null identity(1,1) constraint pk_security_accountsession_id primary key
	, accountID int not null constraint fk_security_accountsession_account 
		foreign key references dxsecurity.account (id)
	, sessionID int not null 
		--constraint fk_security_accountsession_diag_session foreign key references dxdiag.session (id)
	, rv rowversion
)


/*
create table dxxl8.subscriber (
	id int not null identity(1,1) constraint pk_xl8_subscriber_id primary key
	, UID uniqueidentifier not null
	, displayName nvarchar(255) not null
	, createDT datetimeoffset not null constraint df_xl8_subscriber_createdt default getutcdate()
	, enabled tinyint not null constraint def_xl8_subscriber_enabled default 1
)

create table dxxl8.subscriberaccount (
	id int not null identity(1,1) constraint pk_xl8_subscriberaccount_id primary key
	, subscriberID int not null constraint fk_xl8_subscriberaccount_subscriber foreign key references dxxl8.subscriber(ID)
	, accountID int not null constraint fk_xl8_subscriberaccount_security_account foreign key references dxsecurity.account(ID)
	)
	
create table dxxl8.publication (
	id int not null identity(1,1) constraint pk_xl8_publication_id primary key
	, subscriberID int not null constraint fk_xl8_publication_subscriber foreign key references dxxl8.subscriber(ID)
	, UID uniqueidentifier not null
	, displayName nvarchar(255) not null
	, createDT datetimeoffset not null constraint def_xl8_publication_createdt default getutcdate()
)



create table dxxl8.publicationlocale (
	id int not null identity(1,1) constraint pk_xl8_publicationlocale_id primary key
	, publicationID int not null constraint fk_xl8_publicationlocale_publication foreign key references dxxl8.publication(ID)
	, LCID int not null
	, enabled tinyint not null constraint def_xl8_publicationlocale_enabled default 1
	, createDT datetimeoffset not null constraint def_xl8_publicationlocale_createdt default getutcdate()
	, expireDT datetimeoffset null
)
	
create table dxxl8.keyword (
	id int not null identity(1,1) constraint pk_xl8_keyword_id primary key
	, subscriberID int not null constraint fk_xl8_keyword_subscriber foreign key references dxxl8.subscriber(ID)
	, keyword nchar(55) not null
	, createDT datetimeoffset not null constraint df_xl8_keyword_createdt default getutcdate()
)

create table dxxl8.resourcesubscription (
	id int not null identity(1,1) constraint pk_xl8_resourcesubscription_id primary key
	, subscriberID int null constraint fk_xl8_resourcesubscription_subscriber foreign key references dxxl8.subscriber(ID)
	, resourceID int not null constraint fk_xl8_resourcesubscription_resource references dxxl8.resource(ID)
	, createDT datetimeoffset not null constraint df_xl8_resourcesubscription_createdt default getutcdate()
)

create table dxxl8.publicationsubscription (
	id int not null identity constraint pk_xl8_publicationsubscription_id primary key
	, resourceSubscriptionID int not null constraint fk_xl8_pubsub_ressub foreign key references dxxl8.resourcesubscription(ID)
	, publicationID int not null constraint fk_xl8_pubsub_pub foreign key references dxxl8.publication(ID)
	, AKA nchar(55) null
	, createDT datetimeoffset not null constraint def_xl8_pubsub_createdt default getutcdate()
)

create table dxxl8.resourcesubscriptionkeyword (
	id int  not null identity(1,1) constraint pk_xl8_ressubkwd_id primary key
	, resourceSubscriptionID int not null constraint fk_xl8_ressubkwd_ressubid foreign key references dxxl8.resourcesubscription(ID)
	, keywordID int not null constraint fk_xl8_ressubkwd_keywordid foreign key references dxxl8.keyword(ID)
)

create table dxxl8.resourcesubscriptionlocale (
	id int not null identity(1,1) constraint pk_xl8_resourcesubscriptionlocale_id primary key
	, resourceSubscriptionID int not null constraint fk_xl8_ressublcid_ressubid foreign key references dxxl8.resourceSubscription(ID)
	, LCID int not null
)


/*
*/

subscription (application, literal, aka)
*/

create table dxblog.blog (
	id int not null identity(1,1) constraint pk_blog_blog_id primary key
	, UID uniqueidentifier not null
	, name nvarchar(255) not null
	, description nvarchar(255) not null
	, enabled tinyint not null constraint def_blog_blog_enabled default(1)
)


--category (id, guid, name, desc)
create table dxblog.category (
	id int not null identity(1,1) constraint pk_blog_category_id primary key
	, UID uniqueidentifier not null
	, literal_resourceid int not null constraint fk_blog_category_xl8_literal_id foreign key references dxxl8.resource(ID)
	, desc_resourceid int null constraint fk_blog_category_xl8_desc_id foreign key references dxxl8.resource(ID)
)

--post (id, guid, title, description, content, slug, created, modified, ispublished, cancomment, author)
create table dxblog.post (
	id int not null identity(1,1) constraint pk_blog_post_id primary key
	, UID uniqueidentifier not null
	, blogID int not null constraint fk_blog_post_blog_id foreign key references dxblog.blog(ID)
	, title nvarchar(255) not null
	, content nvarchar(max) not null
	, slug nvarchar(255) null
	, createDT datetimeoffset not null constraint def_blog_post_createdt default (getutcdate())
	, authorID int not null --constraint fk_blog_post_security_account_ID foreign key references dxsecurity.account(ID)
	, published bit not null default 0
	, commentEnabled bit not null default 0
)

--postcategory
create table dxblog.postcategory (
	postID int not null constraint fk_blog_post_category_postID foreign key references dxblog.post (id)
	, categoryID int not null constraint fk_blog_post_category_categoryid foreign key references dxblog.category (id)
)
alter table dxblog.postcategory add constraint pk_blog_postcategory primary key (postID, categoryID)

--postcomment
create table dxblog.postcomment (
	id int not null identity(1,1) constraint pk_blog_postcomment_id primary key
	, postID int not null constraint fk_blog_postcomment_post_id foreign key references dxblog.post(id)
	, authorID int not null --constraint fk_blog_postcomment_security_account_id foreign key references dxsecurity.account(id)
	, comment nvarchar(1000) not null
	, createDT datetimeoffset not null constraint def_blog_postcomment_createdt default getutcdate()
	, approved bit not null constraint def_blog_postcomment_approved default 0
)

--posttag (id, postid, tag)
create table dxblog.posttag (
	id int not null identity(1,1) constraint pk_blog_posttag_id primary key
	, postid int not null constraint fk_blog_posttag_post_id foreign key references dxblog.post(id)
	, tag nvarchar(50) not null
)
create index udx_tag on dxblog.posttag (tag)

--referrer (id, datetime, reffererurl, postid, referralurl)
create table dxblog.referral (
	id int not null identity(1,1) constraint pk_blog_referral_id primary key
	, referrerUrl varchar(255) not null
	, url varchar(255) not null
	, referralDT datetimeoffset not null constraint def_blog_referral_dt default getutcdate()
	, isSpam bit not null constraint def_blog_referral_isspam default 0
)

--postrating (id, postid, rating, userid)
create table dxblog.postrating (
	id int not null identity(1,1) constraint pk_blog_postrating_id primary key
	, postID int not null constraint fk_blog_postrating_post_id foreign key references dxblog.post (id)
	, rating int not null
	, accountID int null constraint fk_blog_postrating_security_account_id foreign key references dxsecurity.account(id)
	, deviceAddress nvarchar(255) null
	, createDT datetimeoffset not null constraint def_blog_postrating_createdt default getutcdate()
)


declare @res_user_desc int;
insert into dxxl8.resource values ('386D444B-038A-49BB-91B9-A69E7162608B', GETUTCDATE())
set @res_user_desc = SCOPE_IDENTITY()
insert into dxxl8.xlate values (@res_user_desc, '1033', 'An application end-user', GETUTCDATE())

declare @res_user_lit int
insert into dxxl8.resource values ('59FC3FE6-FFBF-4F21-83D9-6E72A520AF3F', GETUTCDATE())
set @res_user_lit = SCOPE_IDENTITY()
insert into dxxl8.xlate values (@res_user_lit, '1033', 'User', GETUTCDATE())


declare @res_select int
insert into dxxl8.resource values ('10C5A9F3-292F-4A65-8AF8-AA969AFC083F', GETUTCDATE())
set @res_select = SCOPE_IDENTITY()
insert into dxxl8.xlate values (@res_select, '1033', 'select', GETUTCDATE())
insert into dxxl8.xlate values (@res_select, '1036', 'shoisissez', GETUTCDATE())

declare @res_yes int
insert into dxxl8.resource values ('4CA0C8FF-32B0-452C-B576-37F52A85DBC3', GETUTCDATE())
set @res_yes = SCOPE_IDENTITY()
insert into dxxl8.xlate values (@res_yes, '1033', 'yes', GETUTCDATE())
insert into dxxl8.xlate values (@res_yes, '1036', 'oui', GETUTCDATE())

declare @res_no int
insert into dxxl8.resource values ('2BA346E2-92F7-4E19-885A-2E96EF137C70', GETUTCDATE())
set @res_no = SCOPE_IDENTITY()
insert into dxxl8.xlate values (@res_no, '1033', 'no', GETUTCDATE())
insert into dxxl8.xlate values (@res_no, '1036', 'non', GETUTCDATE())

declare @res_usergroup_lit int
insert into dxxl8.resource values ('11A21E3F-1EB4-408E-A9CE-7D304DE31656', GETUTCDATE())
set @res_usergroup_lit = SCOPE_IDENTITY()
insert into dxxl8.xlate values (@res_usergroup_lit, '1033', 'User Group', GETUTCDATE())

declare @res_usergroup_desc int
insert into dxxl8.resource values ('34A14B40-5202-4A55-8268-83BBBF1F7324', GETUTCDATE())
set @res_usergroup_desc = SCOPE_IDENTITY()
insert into dxxl8.xlate values (@res_usergroup_desc, '1033', 'A Group of Users', GETUTCDATE())

declare @objecttype_usergroup int;
insert into dxdimension.objecttype values (@res_usergroup_lit, @res_usergroup_desc)
set @objecttype_usergroup = SCOPE_IDENTITY();

declare @objecttype_user int;
insert into dxdimension.objecttype values (@res_user_lit, @res_user_desc)
set @objecttype_user = SCOPE_IDENTITY();

insert into dxdimension.objecttyperelationship values (@objecttype_usergroup, @objecttype_user, 1)

select l.id, l.uid, x.literal from dxxl8.resource l inner join dxxl8.xlate x on l.ID = x.resourceID
	where x.LCID = 1033
	
	
	
select * from dxsecurity.credential	