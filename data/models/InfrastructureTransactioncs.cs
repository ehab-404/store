namespace testRestApi.data.models
{
    public class InfrastructureTransaction
    {
        public int Id { get; set; }

        public string TableName { get; set; }

        public string OperationType { get; set; } // Insert, Update, Delete

        public string PrimaryKeyValue { get; set; }

        public string EntityDataJson { get; set; }

       // public string TransactionId { get; set; }

        public string?UserName { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsSoftDelete{ get; set; }
        public bool IsBulkOPeration { get; set; }
    }
}
