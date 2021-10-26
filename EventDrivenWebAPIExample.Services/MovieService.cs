using System.Collections.Generic;
using System.Threading.Tasks;
using EventDrivenWebAPIExample.Domain;
using EventDrivenWebAPIExample.Domain.Mongo;
using EventDrivenWebAPIExample.Infrastructure.Interface.Mongo;
using EventDrivenWebAPIExample.Services.Interface;

namespace EventDrivenWebAPIExample.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMongoRepository<Movie> _movieRepository;
        private readonly IKafkaMessengerService _kafkaMessengerService;
        public MovieService(IMongoRepository<Movie> movieRepository, IKafkaMessengerService kafkaMessengerService)
        {
            _movieRepository = movieRepository;
            _kafkaMessengerService = kafkaMessengerService;
        }
        public Task<Movie> FindMovieByTitleAsync(string title)
        {
            return _movieRepository.FindOneAsync(b => b.Title == title);
        }

        public List<KafkaReturnValue> SendMovie(Movie movie)
        {
            Task<List<KafkaReturnValue>> tasks;

            using (tasks = _kafkaMessengerService.SendKafkaMessage(movie.Title, "UpdateMovie", movie))
            {
                tasks.Wait();
            }

            return tasks.Result;
        }
    }
}
