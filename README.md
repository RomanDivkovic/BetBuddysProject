# BetBuddys .NET API

This is a .NET Web API project that serves as a database backend for the BetBuddys application. It provides RESTful endpoints for managing users, groups, events, predictions, posts, and more.

## Features

- **User Management**: Create, read, update, and delete users
- **Group Management**: Create groups, manage members, and handle group settings
- **Event Management**: Create UFC/MMA events with matches
- **Prediction System**: Allow users to make predictions on match outcomes
- **Social Features**: Posts, comments, and reactions within groups
- **Leaderboard**: Track user performance and rankings
- **Invitation System**: Invite users to groups via email
- **Messaging**: Direct messaging between users
- **Notifications**: System notifications for users

## Technology Stack

- .NET 8.0
- Entity Framework Core (SQLite for development)
- ASP.NET Core Web API
- Swagger/OpenAPI for API documentation

## Database Models

### Core Models
- `User`: User account information
- `Group`: Betting groups with members
- `GroupMember`: Join table for group membership
- `Event`: UFC/MMA events
- `Match`: Individual fights within events
- `Prediction`: User predictions on match outcomes
- `LeaderboardEntry`: User scoring and rankings

### Social Models
- `Post`: Group posts
- `Comment`: Comments on posts
- `PostReaction`: Likes/dislikes on posts
- `Invitation`: Group invitations
- `Message`: Direct messages between users
- `Conversation`: Message threads
- `Notification`: System notifications

### Additional Models
- `Series`: Event series (tournaments)
- `Bet`: Custom betting options
- `BetOption`: Betting choices
- `UserBet`: User bet selections

## API Endpoints

### Users
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `GET /api/users/email/{email}` - Get user by email
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

### Groups
- `GET /api/groups/{id}` - Get group by ID
- `GET /api/groups/user/{userId}` - Get user's groups
- `POST /api/groups` - Create new group
- `PUT /api/groups/{id}` - Update group
- `DELETE /api/groups/{id}` - Delete group
- `GET /api/groups/{id}/members` - Get group members
- `POST /api/groups/{groupId}/members/{userId}` - Add member
- `DELETE /api/groups/{groupId}/members/{userId}` - Remove member

### Events
- `GET /api/events/{id}` - Get event by ID
- `GET /api/events/group/{groupId}` - Get group events
- `POST /api/events` - Create new event
- `PUT /api/events/{id}` - Update event
- `DELETE /api/events/{id}` - Delete event
- `GET /api/events/{eventId}/matches` - Get event matches
- `POST /api/events/matches` - Create match
- `PUT /api/events/matches/{matchId}` - Update match

### Predictions
- `GET /api/predictions/{id}` - Get prediction by ID
- `GET /api/predictions/user/{userId}/event/{eventId}` - Get user predictions
- `GET /api/predictions/match/{matchId}` - Get match predictions
- `POST /api/predictions` - Create prediction
- `PUT /api/predictions/{id}` - Update prediction
- `POST /api/predictions/matches/{matchId}/score` - Score predictions

### Posts
- `GET /api/posts/{id}` - Get post by ID
- `GET /api/posts/group/{groupId}` - Get group posts
- `POST /api/posts` - Create post
- `PUT /api/posts/{id}` - Update post
- `DELETE /api/posts/{id}` - Delete post
- `GET /api/posts/{postId}/comments` - Get post comments
- `POST /api/posts/comments` - Create comment
- `POST /api/posts/{postId}/react` - React to post

### Leaderboard
- `GET /api/leaderboard/group/{groupId}` - Get group leaderboard
- `GET /api/leaderboard/event/{eventId}` - Get event leaderboard
- `GET /api/leaderboard/user/{userId}/group/{groupId}` - Get user leaderboard entry
- `POST /api/leaderboard/recalculate/group/{groupId}` - Recalculate leaderboard

### Invitations
- `GET /api/invitations/{id}` - Get invitation by ID
- `GET /api/invitations/user/{userEmail}` - Get user invitations
- `GET /api/invitations/group/{groupId}` - Get group invitations
- `POST /api/invitations` - Create invitation
- `POST /api/invitations/{id}/respond` - Respond to invitation

## Setup and Installation

1. **Prerequisites**
   - .NET 8.0 SDK
   - SQL Server (LocalDB or full SQL Server)
   - Visual Studio Code or Visual Studio

2. **Database Setup**
   - The project uses SQLite for development (no additional setup required)
   - For production, you can switch to SQL Server by updating the connection string and changing the EF provider
   - The database will be created automatically on first run

3. **Run the Application**
   ```bash
   dotnet restore
   dotnet run
   ```

4. **API Documentation**
   - Navigate to `https://localhost:5001/swagger` (or the appropriate URL) to view the Swagger UI
   - Use the Swagger interface to test API endpoints

## Configuration

### Database Connection
The project uses SQLite by default for development:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=betbuddys.db"
  }
}
```

For production with SQL Server, update to:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BetBuddysDb;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```
And change the DbContext configuration in `Program.cs` to use `UseSqlServer()` instead of `UseSqlite()`.

### CORS
The API is configured to allow all origins in development. For production, update the CORS policy in `Program.cs`.

## Integration with Frontend

This API is designed to work with the BetBuddys React/TypeScript frontend. The models and endpoints mirror the data structures used in the Firebase implementation, making it easy to migrate or run in parallel.

### Key Differences from Firebase
- Uses SQLite/SQL Server instead of Firebase Realtime Database
- Provides structured relational data models
- Offers more robust querying capabilities
- Better performance for complex data operations
- Easier to backup and migrate data

## Development Notes

- The API uses Entity Framework Code First approach
- Database migrations are handled automatically on startup
- All endpoints return JSON responses
- Error handling is implemented at the controller level
- Services are dependency injected for better testability

## Future Enhancements

- Add authentication middleware (JWT tokens)
- Implement pagination for large data sets
- Add caching for frequently accessed data
- Include comprehensive logging
- Add unit and integration tests
- Implement background jobs for scoring and notifications
