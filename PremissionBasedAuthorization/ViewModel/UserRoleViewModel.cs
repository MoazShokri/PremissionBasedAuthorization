namespace PremissionBasedAuthorization.ViewModel

{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<CheckBoxViewModel> Roles { get; set; }
    }
}
