using ProtoBuf.Grpc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace BMC.Models
{
    #region auth
    [DataContract]
    public class AuthenticationModel
    {
        [DataMember(Order = 1)]
        public string ApiKey { get; set; }
    }
    [DataContract]
    public class AuthenticationUserModel
    {
        [DataMember(Order = 1)]
        public string Username { get; set; }
        [DataMember(Order = 2)]
        public string Password { get; set; }
    }
    [DataContract]
    public class AuthenticatedUserModel
    {
        [DataMember(Order = 1)]
        public string Username { get; set; }
        [DataMember(Order = 2)]
        public string AccessToken { get; set; }
        [DataMember(Order = 3)]
        public string TokenType { get; set; }
        [DataMember(Order = 4)]
        public DateTime? ExpiredDate { get; set; }
    }
    #endregion
    #region GRPC
    [ServiceContract]
    public interface IAuth
    {
        [OperationContract]
        Task<AuthenticatedUserModel> AuthenticateWithUsername(AuthenticationUserModel data, CallContext context = default);

        [OperationContract]
        Task<AuthenticatedUserModel> AuthenticateWithApiKey(AuthenticationModel data, CallContext context = default);
    }
    [ServiceContract]
    public interface IMqttTopic : ICrudGrpc<MqttTopic>
    {

    }
    [ServiceContract]
    public interface IProject : ICrudGrpc<Project>
    {

    }
    [ServiceContract]
    public interface IDashboard : ICrudGrpc<Dashboard>
    {

    }
    [ServiceContract]
    public interface IAlert : ICrudGrpc<Alert>
    {

    } 
    
    [ServiceContract]
    public interface IDevice : ICrudGrpc<Device>
    {

    }
    
    [ServiceContract]
    public interface IMessageStream : ICrudGrpc<MessageStream>
    {

    }
    [ServiceContract]
    public interface IUserProfile : ICrudGrpc<UserProfile>
    {
        [OperationContract]
        Task<UserProfile> GetItemByEmail(InputCls input, CallContext context = default);

        [OperationContract]
        Task<UserProfile> GetItemByPhone(InputCls input, CallContext context = default);

        [OperationContract]
        Task<OutputCls> IsUserExists(InputCls input, CallContext context = default);

        [OperationContract]
        Task<OutputCls> GetUserRole(InputCls input, CallContext context = default);
    }
    #endregion

    #region Common
    public interface ICrud<T> where T : class
    {
        Task<bool> InsertData(T data);
        Task<bool> UpdateData(T data);
        Task<List<T>> GetAllData();
        Task<T> GetDataById(long Id);
        Task<bool> DeleteData(long Id);
        Task<long> GetLastId();
        Task<List<T>> FindByKeyword(string Keyword);
    }
    [ServiceContract]
    public interface ICrudGrpc<T> where T : class
    {
        [OperationContract]
        Task<OutputCls> InsertData(T data, CallContext context = default);
        [OperationContract]
        Task<OutputCls> UpdateData(T data, CallContext context = default);
        [OperationContract]
        Task<List<T>> GetAllData(CallContext context = default);
        [OperationContract]
        Task<T> GetDataById(InputCls Id, CallContext context = default);
        [OperationContract]
        Task<OutputCls> DeleteData(InputCls Id, CallContext context = default);
        [OperationContract]
        Task<OutputCls> GetLastId(CallContext context = default);
        [OperationContract]
        Task<List<T>> FindByKeyword(string Keyword, CallContext context = default);
    }
    [DataContract]
    public class InputCls
    {
        [DataMember(Order = 1)]
        public string[] Param { get; set; }
        [DataMember(Order = 2)]
        public Type[] ParamType { get; set; }
    }
    [DataContract]
    public class OutputCls
    {
        [DataMember(Order = 1)]
        public bool Result { get; set; }
        [DataMember(Order = 2)]
        public string Message { get; set; }
        [DataMember(Order = 3)]
        public string Data { get; set; }
    }
    #endregion
    #region database

    [DataContract]
    public class Project
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [DataMember(Order = 2)]
        public string? Name { set; get; }
        [DataMember(Order = 3)]
        public string? Desc { set; get; }
        [DataMember(Order = 4)]
        public DateTime? StartDate { set; get; }
        [DataMember(Order = 5)]
        public string? Pic { set; get; }
        [DataMember(Order = 6)]
        public string? Email { set; get; }
        [DataMember(Order = 7)]
        [Required]
        public string Username { set; get; }

        public ICollection<MqttTopic> MqttTopics { get; set; }
        public ICollection<Dashboard> Dashboards { get; set; }
    }

    [DataContract]
    public class MqttTopic
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [DataMember(Order = 2)]
        [ForeignKey("Project")]
        public long ProjectId { set; get; }
        [DataMember(Order = 3)]
        public string? Topic { set; get; }
        [DataMember(Order = 4)]
        public string? Desc { set; get; }
        [DataMember(Order = 5)]
        public string? JsonTemplate { set; get; }
        [DataMember(Order = 6)]
        [Required]
        public string Username { set; get; }

        public Project Project { set; get; }

        public ICollection<Alert> Alerts { get; set; }
    }

    public enum ChartTypes { Line, Bar, Area }

    [DataContract]
    public class Dashboard
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [DataMember(Order = 2)]
        [ForeignKey("Project")]
        public long ProjectId { set; get; }
        [DataMember(Order = 3)]
        public string? Name { set; get; }
        [DataMember(Order = 4)]
        public string? Desc { set; get; }
        [DataMember(Order = 5)]
        public ChartTypes ChartType { set; get; }
        [DataMember(Order = 6)]
        public string? XAxisMember { set; get; }
        [DataMember(Order = 7)]
        public string? YAxisMember { set; get; }
        [DataMember(Order = 8)]
        public string? XAxisType { set; get; }
        [DataMember(Order = 9)]
        public string? YAxisType { set; get; }
        [DataMember(Order = 10)]
        [Required]
        public string Username { set; get; } 
        
        [DataMember(Order = 11)]
        public string? DashboardUrl { set; get; }

        [DataMember(Order = 12)]
        public string? MqttTopic { set; get; }

        public Project Project { set; get; }
    }
    [DataContract]
    public class Alert
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [DataMember(Order = 2)]
        [ForeignKey("MqttTopic")]
        public long MqttTopicId { set; get; }
        [DataMember(Order = 3)]
        public string? Name { set; get; }
        
        [DataMember(Order = 4)]
        public string? FilterQuery { set; get; }
        [DataMember(Order = 5)]
        public string? MessageTemplate { set; get; }
        [DataMember(Order = 6)]
        public string? SendToEmail { set; get; }
        [DataMember(Order = 7)]
        public string? SendToPhone { set; get; }
        [DataMember(Order = 8)]
        public string? CallUrl { set; get; }
        [DataMember(Order = 9)]
        [Required]
        public string Username { set; get; }
        public MqttTopic MqttTopic { set; get; }
    } 
    
    [DataContract]
    public class Device
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [DataMember(Order = 2)]
        [Required]
        public string MqttClientId { set; get; }
        [DataMember(Order = 3)]
        public string? Name { set; get; }
        [DataMember(Order = 4)]
        public string? Desc { set; get; }
        [DataMember(Order = 5)]
        public DeviceTypes DeviceType { set; get; }
     
        [DataMember(Order = 6)]
        [Required]
        public string Username { set; get; }
    } 
    
    [DataContract]
    public class MessageStream
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [DataMember(Order = 2)]
       
        public DateTime? CreatedDate { set; get; }
        [DataMember(Order = 3)]
        public string? Content { set; get; }
        [DataMember(Order = 4)]
        public string? MqttClientId { set; get; }
        [DataMember(Order = 5)]
        public string? MqttTopic { set; get; }     
        
        [DataMember(Order = 6)]
        public string? Username { set; get; }     
     
    }

    public enum DeviceTypes { Sensor, Actuator, Logger, NetworkDevice}

    [DataContract]
    public class UserProfile
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [DataMember(Order = 2)]
        public string Username { get; set; }
        [DataMember(Order = 3)]
        public string Password { get; set; }
        [DataMember(Order = 4)]
        public string FullName { get; set; }
        [DataMember(Order = 5)]
        public string? Phone { get; set; }
        [DataMember(Order = 6)]
        public string? Email { get; set; }
        [DataMember(Order = 7)]
        public string? Alamat { get; set; }
        [DataMember(Order = 8)]
        public string? KTP { get; set; }
        [DataMember(Order = 9)]
        public string? PicUrl { get; set; }
        [DataMember(Order = 10)]
        public bool Aktif { get; set; } = true;

        [DataMember(Order = 11)]
        public Roles Role { set; get; } = Roles.User;

    }
    public class DataSeriesItem
    {
        public double NilaiY { get; set; }
        public string NilaiX { get; set; }
    }
    public enum Roles { Admin, User, Operator }
    #endregion
}