namespace SkillSnap.Client.Services
{
    public class UserSessionService
    {
        private string? _userId;
        private string? _userRole;
        private bool _isEditing;

        public string? UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnStateChanged?.Invoke();
            }
        }

        public string? UserRole
        {
            get => _userRole;
            set
            {
                _userRole = value;
                OnStateChanged?.Invoke();
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnStateChanged?.Invoke();
            }
        }

        public event Action? OnStateChanged;

        public void ClearSession()
        {
            _userId = null;
            _userRole = null;
            _isEditing = false;
            OnStateChanged?.Invoke();
        }
    }
}
