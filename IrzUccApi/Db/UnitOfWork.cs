using IrzUccApi.Db.Repositories;

namespace IrzUccApi.Db
{
    public class UnitOfWork
    {
        private readonly AppDbContext _dbContext;

        private CabinetRepository? _cabinetRepository;
        private ChatRepository? _chatRepository;
        private CommentRepository? _commentRepository;
        private EventRepository? _eventRepository;
        private ImageRepository? _imageRepository;
        private MessageRepository? _messageRepository;
        private NewsEntryRepository? _newsEntryRepository;
        private PositionRepository? _positionRepository;
        private RoleRepository? _roleRepository;
        private UserPositionRepository? _userPositionRepository;
        private UserRepository? _userRepository;
        private UserRoleRepository? _userRoleRepository;

        public CabinetRepository Cabinets
        {
            get
            {
                _cabinetRepository ??= new CabinetRepository(_dbContext);
                return _cabinetRepository;
            }
        }
        public ChatRepository Chats
        {
            get
            {
                _chatRepository ??= new ChatRepository(_dbContext);
                return _chatRepository;
            }
        }
        public CommentRepository Comments
        {
            get
            {
                _commentRepository ??= new CommentRepository(_dbContext);
                return _commentRepository;
            }
        }
        public EventRepository Events
        {
            get
            {
                _eventRepository ??= new EventRepository(_dbContext);
                return _eventRepository;
            }
        }
        public ImageRepository Images
        {
            get
            {
                _imageRepository ??= new ImageRepository(_dbContext);
                return _imageRepository;
            }
        }
        public MessageRepository Messages
        {
            get
            {
                _messageRepository ??= new MessageRepository(_dbContext);
                return _messageRepository;
            }
        }
        public NewsEntryRepository NewsEntries
        {
            get
            {
                _newsEntryRepository ??= new NewsEntryRepository(_dbContext);
                return _newsEntryRepository;
            }
        }
        public PositionRepository Positions
        {
            get
            {
                _positionRepository ??= new PositionRepository(_dbContext);
                return _positionRepository;
            }
        }
        public RoleRepository Roles
        {
            get
            {
                _roleRepository ??= new RoleRepository(_dbContext);
                return _roleRepository;
            }
        }
        public UserPositionRepository UserPositions
        {
            get
            {
                _userPositionRepository ??= new UserPositionRepository(_dbContext);
                return _userPositionRepository;
            }
        }
        public UserRepository Users
        {
            get
            {
                _userRepository ??= new UserRepository(_dbContext);
                return _userRepository;
            }
        }

        public UserRoleRepository UserRole
        {
            get
            {
                _userRoleRepository ??= new UserRoleRepository(_dbContext);
                return _userRoleRepository;
            }
        }

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveAsync()
            => await _dbContext.SaveChangesAsync();
    }
}
