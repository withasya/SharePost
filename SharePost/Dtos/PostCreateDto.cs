namespace SharePost.Dtos
{
    public class PostCreateDto
    {
        public required IFormFile Image { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
