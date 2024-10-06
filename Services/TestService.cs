// using Microsoft.EntityFrameworkCore;
// using banbet.Data;


// namespace banbet.Services
// {
//     public class TestService
//     {
//         private readonly ILogger<TestService> _logger;
//         private readonly ApplicationDbContext _dbContext;
//         public TestService(ILogger<TestService> logger, ApplicationDbContext dbContext)
//         {
//             _dbContext = dbContext;
//             _logger = logger;
//         }


//         public void LogSomething()
//         {
//             _dbContext.Users.Add(new Model.User 
//             {
//                 Id = 123
//             });
//             _dbContext.SaveChanges();
//             _logger.LogInformation("OIAJSDOIJASIODJASIJD");
//         }
//     }
// }