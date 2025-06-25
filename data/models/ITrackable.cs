namespace testRestApi.data.models
{
    public interface ITrackable

    {
        DateTime? CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }
}
