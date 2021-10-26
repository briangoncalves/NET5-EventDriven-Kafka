using System.Linq;
using System.Threading.Tasks;
using EventDrivenWebAPIExample.Kafka.Domain.Mongo;
using EventDrivenWebAPIExample.Kafka.Infrastructure.Interface.Mongo;
using EventDrivenWebAPIExample.Kafka.Services.Interface;

namespace EventDrivenWebAPIExample.Kafka.Services
{
    public class MovieKafkaExecutor : IKafkaExecutor<Movie>
	{
		private readonly IMongoRepository<Movie> _movieRepository;

		public MovieKafkaExecutor(IMongoRepository<Movie> movieRepository)
		{
			_movieRepository = movieRepository;
		}

		public async Task<bool> Execute(Movie movie, string subject)
		{
			var found = _movieRepository.AsQueryable().FirstOrDefault(m => m.Title == movie.Title);
			if (found is null)
            {
				await _movieRepository.InsertOneAsync(movie);				
            } else
            {
				found.Description = movie.Description;
				found.Category = movie.Category;
				await _movieRepository.ReplaceOneAsync(found);
            }
			return true;
		}
	}
}
