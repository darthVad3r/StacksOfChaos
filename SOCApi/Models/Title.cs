using Newtonsoft.Json;

namespace SOCApi.Models
{
    public class Title
    {
        [JsonProperty("numFound")]
        public int NumFound { get; set; }
        [JsonProperty("start")]
        public int Start { get; set; }
        [JsonProperty("numFoundExact")]
        public bool NumFoundExact { get; set; }
        [JsonProperty("docs")]
        public Doc[]? Docs { get; set; }
        //public int num_found { get; set; }
        [JsonProperty("q")]
        public string? Q { get; set; }
        [JsonProperty("offset")]
        public object? Offset { get; set; }
    }

    public class Doc
    {
        [JsonProperty("author_key")]
        public string[]? AuthorKey { get; set; }
        [JsonProperty("author_name")]
        public string[]? AuthorName { get; set; }
        [JsonProperty("cover_edition_key")]
        public string? CoverEditionKey { get; set; }
        [JsonProperty("cover_i")]
        public int CoverI { get; set; }
        [JsonProperty("ebook_access")]
        public string? EbookAccess { get; set; }
        [JsonProperty("ebook_count_i")]
        public int EbookCountI { get; set; }
        [JsonProperty("edition_count")]
        public int EditionCount { get; set; }
        [JsonProperty("edition_key")]
        public string[]? EditionKey { get; set; }
        [JsonProperty("first_publish_year")]
        public int FirstPublishYear { get; set; }
        [JsonProperty("format")]
        public string[]? Format { get; set; }
        [JsonProperty("has_fulltext")]
        public bool HasFullText { get; set; }
        [JsonProperty("ia")]
        public string[]? Ia { get; set; }
        [JsonProperty("isbn")]
        public string[]? Isbn { get; set; }
        [JsonProperty("key")]
        public string? Key { get; set; }
        [JsonProperty("language")]
        public string[]? Language { get; set; }
        [JsonProperty("last_modified_i")]
        public int LastModifiedI { get; set; }
        [JsonProperty("lcc")]
        public string[]? Lcc { get; set; }
        [JsonProperty("lccn")]
        public string[]? Lccn { get; set; }
        [JsonProperty("number_of_pages_median")]
        public int MedianNumberOfPages { get; set; }
        [JsonProperty("oclc")]
        public string[]? Oclc { get; set; }
        [JsonProperty("public_scan_b")]
        public bool PublishScanB { get; set; }
        [JsonProperty("publish_date")]
        public string[]? PublishDate { get; set; }
        [JsonProperty("publish_year")]
        public int[]? PublishYear { get; set; }
        [JsonProperty("publisher")]
        public string[]? Publisher { get; set; }
        [JsonProperty("seed")]
        public string[]? Seed { get; set; }
        [JsonProperty("title")]
        public string? Title { get; set; }
        [JsonProperty("title_sort")]
        public string? TitleSort { get; set; }
        [JsonProperty("title_suggest")]
        public string? TitleSuggest { get; set; }
        [JsonProperty("type")]
        public string? Type { get; set; }
        [JsonProperty("id_amazon")]
        public string[]? AmazonId { get; set; }
        [JsonProperty("subject")]
        public string[]? Subject { get; set; }
        [JsonProperty("place")]
        public string[]? Place { get; set; }
        [JsonProperty("time")]
        public string[]? Time { get; set; }
        [JsonProperty("person")]
        public string[]? Person { get; set; }
        [JsonProperty("ratings_average")]
        public float AverageRatings { get; set; }
        [JsonProperty("ratings_sortable")]
        public float SortableRatings { get; set; }
        [JsonProperty("ratings_count")]
        public int RatingsCount { get; set; }
        [JsonProperty("ratings_count_1")]
        public int RatingsCount1 { get; set; }
        [JsonProperty("ratings_count_2")]
        public int RatingsCount2 { get; set; }
        [JsonProperty("ratings_count_3")]
        public int RatingsCount3 { get; set; }
        [JsonProperty("ratings_count_4")]
        public int RatingsCount4 { get; set; }
        [JsonProperty("ratings_count_5")]
        public int RatingsCount5 { get; set; }
        [JsonProperty("readinglog_count")]
        public int ReadingLogCount { get; set; }
        [JsonProperty("want_to_read_count")]
        public int WantToReadCount { get; set; }
        [JsonProperty("currently_reading_count")]
        public int CurrentlyReadingCount { get; set; }
        [JsonProperty("already_read_count")]
        public int AlreadyReadCount { get; set; }
        [JsonProperty("publisher_facet")]
        public string[]? PublisherFacet { get; set; }
        [JsonProperty("person_key")]
        public string[]? PersonKey { get; set; }
        [JsonProperty("time_facet")]
        public string[]? TimeFacet { get; set; }
        [JsonProperty("place_key")]
        public string[]? PlaceKey { get; set; }
        [JsonProperty("person_facet")]
        public string[]? PersonFacet { get; set; }
        [JsonProperty("subject_facet")]
        public string[]? SubjectFacet { get; set; }
        [JsonProperty("_version_")]
        public long Version_ { get; set; }
        [JsonProperty("place_facet")]
        public string[]? PlaceFacet { get; set; }
        [JsonProperty("lcc_sort")]
        public string? LccSort { get; set; }
        [JsonProperty("author_facet")]
        public string[]? AuthorFacet { get; set; }
        [JsonProperty("subject_key")]
        public string[]? SubjectKey { get; set; }
        [JsonProperty("time_key")]
        public string[]? TimeKey { get; set; }
        [JsonProperty("ddc")]
        public string[]? Ddc { get; set; }
        [JsonProperty("ia_collection")]
        public string[]? IaCollection { get; set; }
        [JsonProperty("ia_collection_s")]
        public string? IaCollectionS { get; set; }
        [JsonProperty("ddc_sort")]
        public string? DdcSort { get; set; }
        [JsonProperty("printdisabled_s")]
        public string? PrintDisabledS { get; set; }
        [JsonProperty("contributor")]
        public string[]? Contributor { get; set; }
        [JsonProperty("publish_place")]
        public string[]? PublishPlace { get; set; }
        [JsonProperty("author_alternative_name")]
        public string[]? AuthorAlternativeName { get; set; }
        [JsonProperty("first_sentence")]
        public string[]? FirstSentence { get; set; }
        [JsonProperty("lending_edition_s")]
        public string? LendingEditionS { get; set; }
        [JsonProperty("lending_identifier_s")]
        public string? LendingIdentifierS { get; set; }
        [JsonProperty("osp_count")]
        public int OspCount { get; set; }
        [JsonProperty("id_better_world_books")]
        public string[]? BetterWorldBooksId { get; set; }
        [JsonProperty("id_overdrive")]
        public string[]? OverdriveId { get; set; }
        [JsonProperty("id_librarything")]
        public string[]? LibrarythingId { get; set; }
        [JsonProperty("id_goodreads")]
        public string[]? GoodreadsId { get; set; }
        [JsonProperty("ia_loaded_id")]
        public string[]? IaLoadedId { get; set; }
        [JsonProperty("ia_box_id")]
        public string[]? IaBoxId { get; set; }
        [JsonProperty("id_librivox")]
        public string[]? LibrivoxId { get; set; }
        [JsonProperty("id_project_gutenberg")]
        public string[]? ProjectGutenbergId { get; set; }
        [JsonProperty("id_dnb")]
        public string[]? DnbId { get; set; }
        [JsonProperty("id_google")]
        public string[]? GoogleId { get; set; }
        [JsonProperty("id_standard_ebooks")]
        public string[]? StandardEbooksId { get; set; }
        [JsonProperty("id_depósito_legal")]
        public string[]? DepósitoLegalId { get; set; }
        [JsonProperty("id_wikidata")]
        public string[]? WikidataId { get; set; }
        [JsonProperty("id_hathi_trust")]
        public string[]? HathiTrustId { get; set; }
        [JsonProperty("id_doi")]
        public string[]? DoiId { get; set; }
        [JsonProperty("id_alibris_id")]
        public string[]? AlibrisId { get; set; }
        [JsonProperty("id_paperback_swap")]
        public string[]? PaperbackSwapId { get; set; }
    }
}
