namespace Domain.Sneakers;

public class SneakerImage
{
    public SneakerImageId Id { get; }
    public SneakerId SneakerId { get; }
    public Sneaker? Sneaker { get; }
    
    public string S3Path { get; set; }  // Шлях до файлу в S3

    private SneakerImage(SneakerImageId id, SneakerId sneakerId, string s3Path)
    {
        Id = id;
        SneakerId = sneakerId;
        S3Path = s3Path;  // Ініціалізація шляху до файлу в S3

    }
    
    public void UpdateImageUrl(string newS3Path)
    {
        if (string.IsNullOrWhiteSpace(newS3Path))
        {
            throw new ArgumentException("New image URL cannot be empty", nameof(newS3Path));
        }

        S3Path = newS3Path;
    }

    public static SneakerImage New(SneakerImageId id, SneakerId sneakerId, string s3Path)
    {
        return new SneakerImage(id, sneakerId, s3Path);
    }

    public string FilePath => $"{SneakerId}/{Id}.png";
}