﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("main_info")]
        public class OtherParticipant
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Column("id")] public long Id { get; set; }
            [Column("code_bin")] public long CodeBin { get; set; }
            [Column("first_director")] public string FirstDirector { get; set; }
            [Column("telephone")] public string Telephone { get; set; }
            [Column("mail")] public string Mail { get; set; }
            [Column("web_site")] public string WebSite { get; set; }
            [Column("link_id")] public string LinkId { get; set; }
            public List<ExchangeDoc> ExchangeDocs { get; set; }
            public List<BoardOfDirector> BoardOfDirectors { get; set; }
            public List<Shareholder> Shareholders { get; set; }
            
            public List<AffiliatedPerson> AffiliatedPersons { get; set; }
            public List<OrgType> OrgTypes { get; set; }
            public List<RelationByContact> RelationsByContacts { get; set; }
            public List<RelationByReport> RelationByReports { get; set; }
            public List<Executor> Executors { get; set; }
            public Acccountant Acccountant { get; set; }

        }
        [Table("exchange_docs")]
        public class ExchangeDoc
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Column("id")] public long Id { get; set; }
            [Column("comp_id")] public long CompId { get; set; }
            [Column("code_cb")] public string CodeCb { get; set; }
            [Column("isin")] public string Isin { get; set; }
            [Column("nin")] public string Nin { get; set; }
            [Column("cb_type")] public string CbType { get; set; }
            [Column("include_date")] public DateTime? IncludeDate { get; set; }
            [Column("exclude_date")] public DateTime? ExcludeDate { get; set; }

        }
        [Table("org_types")]
        public class OrgType
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Column("id")] public long Id { get; set; }
            [Column("comp_id")] public long CompId { get; set; }
            [Column("type")] public string Type { get; set; }
            [Column("start_date")] public DateTime? StartDate { get; set; }
            [Column("end_date")] public DateTime? EndDate { get; set; }

        }
        [Table("relations_by_reports")]
        public class RelationByReport
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Column("id")] public long Id { get; set; }
            [Column("comp_id")] public long CompId { get; set; }
            [Column("tied_comp")] public string TiedComp { get; set; }
            [Column("relation_link")] public string RelationLink { get; set; }
            [Column("relation_type")] public long RelationType { get; set; }
            [Column("type")] public string Type { get; set; }
            [Column("year")] public string Year { get; set; }
            [Column("upload_date")] public DateTime? UploadDate { get; set; }
            [Column("section")] public string Section { get; set; }
            [Column("full_name")] public string FullName { get; set; }

        }
        [Table("relations_by_contacts")]
        public class RelationByContact
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Column("id")] public long Id { get; set; }
            [Column("comp_id")] public long CompId { get; set; }
            [Column("tied_comp")] public string TiedComp { get; set; }
            [Column("relation_link")] public string RelationLink { get; set; }
            [Column("relation_type")] public long RelationType { get; set; }
            [Column("full_name")] public string FullName { get; set; }

        }
        [Table("board_of_directors")]
        public class BoardOfDirector
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Column("id")] public long Id { get; set; }
            [Column("comp_id")] public long CompId { get; set; }
            [Column("decision_number")] public string DecisionNumber { get; set; }
            [Column("full_name")] public string FullName { get; set; }
            [Column("decision_date")] public DateTime? DecisionDate { get; set; }
            [Column("report_date")] public string ReportDate { get; set; }
            [Column("iin")] public long? Iin { get; set; }
            
        }
        [Table("executive_agency")]
        public class Executor
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Column("id")] public long Id { get; set; }
            [Column("comp_id")] public long CompId { get; set; }
            [Column("decision_number")] public string decisionNumber { get; set; }
            [Column("full_name")] public string FullName { get; set; }
            [Column("decision_date")] public DateTime? DecisionDate { get; set; }
            [Column("report_date")] public string ReportDate { get; set; }
            [Column("iin")] public long? Iin { get; set; }
        }
        [Table("shareholders")]
        public class Shareholder
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Column("id")] public long Id { get; set; }
            [Column("comp_id")] public long CompId { get; set; }
            [Column("is_individual")] public bool IsIndividual { get; set; }
            [Column("full_name")] public string FullName { get; set; }
            [Column("is_resident")] public bool IsResident { get; set; }
            [Column("share")] public string Share { get; set; }
            [Column("report_date")] public string ReportDate { get; set; }
            [Column("biin")] public long? Biin { get; set; }

        }
        [Table("affiliated_persons")]
        public class AffiliatedPerson
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Column("id")] public long Id { get; set; }
            [Column("comp_id")] public long CompId { get; set; }
            [Column("is_individual")] public bool IsIndividual { get; set; }
            [Column("full_name")] public string FullName { get; set; }
            [Column("affiliation_date")] public DateTime? AffiliationDate { get; set; }
            [Column("birth_date")] public DateTime? BirthDate { get; set; }
            [Column("reg_date")] public DateTime? RegDate { get; set; }
            [Column("reg_num")] public string RegNum { get; set; }
            [Column("basis_for_affiliation")] public string BasisForAffiliation { get; set; }
            [Column("report_date")] public string ReportDate { get; set; }
            [Column("biin")] public long? Biin { get; set; }

        }
        
        [Table("accountants")]
        public class Acccountant
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Column("id")] public long Id { get; set; }
            [Column("comp_id")] public long CompId { get; set; }
            [Column("full_name")] public string FullName { get; set; }
            [Column("certifier")] public string Certifier { get; set; }
            [Column("certificate_number")] public string CertificateNumber { get; set; }
            [Column("certification_date")] public DateTime? CertificationDate { get; set; }
            [Column("accountant_organization")] public string AccountantOrganization { get; set; }
            [Column("membership_card")] public string MembershipCard { get; set; }
            [Column("join_date")] public DateTime? JoinDate { get; set; }
            [Column("iin")] public long? Iin { get; set; }
            
    }
}