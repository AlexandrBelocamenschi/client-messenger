using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;


public class Program
{
  public static HttpClient client;
  public static string ip;
  public static async Task Main(string[] args)
  {
    HttpClientHandler clientHandler = new HttpClientHandler();
    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
    client = new HttpClient(clientHandler);

    Console.Write("Program.ip to connect to: ");
    Program.ip = Console.ReadLine();
    //Program.ip = "https://192.168.11.57:5171";
    Console.WriteLine($"https://{Program.ip}:5171/Chat/all-users-chats");
    //return;
    Console.Write("Token: ");
    string token = Console.ReadLine();

    Console.WriteLine("All Chats:");
    UserInfo userInfo = new UserInfo{
      SessionToken = token
    };
    try{
      string jsonRequest = JsonSerializer.Serialize(userInfo);
      string jsonResponse = await PostRequestAsync($"https://{Program.ip}:5171/Chat/all-users-chats", jsonRequest);
      var options = new JsonSerializerOptions{
        PropertyNameCaseInsensitive = true
      };
      ChatsResponseData response = JsonSerializer.Deserialize<ChatsResponseData>(jsonResponse, options);
      if (response == null){
        Console.WriteLine("Ошибка: Не удалось распарсить ответ.");
      }
      else if (response.status == "success" && response.data?.chats != null){
        foreach(var chat in response.data.chats){
          Console.WriteLine("Id: "+ chat.ChatID + " Chat Name: " + chat.ChatName);
        }
      }
      else if (response.status == "error"){
        Console.WriteLine("Сервер вернул ошибку: " + response.error);
      }
      else{
        Console.WriteLine("Неизвестный ответ от сервера.");
      }
    }
    catch (Exception ex){
      Console.WriteLine("Ошибка: " + ex.Message);
    }

    string command = "";
    while(true){
      Console.Write("Write command: ");
      command = Console.ReadLine();
      switch(command){
        case "/chat":
          System.Console.Write("Write chatId: ");
          string chatID = Console.ReadLine();
          _ = Task.Run(() => GetLastMessagesInChat(token, chatID));
          _ = Task.Run(() => GetNewMessages(token, chatID));
          while (true){
            string data = Console.ReadLine();
            if(data == "/exit"){
              break;
            }
            MessageInfo UserMessage = new MessageInfo{
              SessionToken = token,
              Content = data,
              ChatID = chatID
            };
            try{
              string jsonRequest = JsonSerializer.Serialize(UserMessage);
              string jsonResponse = await PostRequestAsync($"https://{Program.ip}:5171/Message/save", jsonRequest);
            }
            catch (Exception ex){
              Console.WriteLine("Ошибка: " + ex.Message);
            }
          }
          break;
        case "/new-chat":
          Console.Write("Write chat name: ");
          string chatName = Console.ReadLine();
          ChatInfoType1 chatInfoType1 = new ChatInfoType1{
            SessionToken = token,
            ChatName = chatName
          };
          try{
            string jsonRequest = JsonSerializer.Serialize(chatInfoType1);
            string jsonResponse = await PostRequestAsync($"https://{Program.ip}:5171/Chat/new-chat", jsonRequest);
            var options = new JsonSerializerOptions{
              PropertyNameCaseInsensitive = true
            };
            ChatResponseData response = JsonSerializer.Deserialize<ChatResponseData>(jsonResponse, options);
            if (response == null){
              Console.WriteLine("Ошибка: Не удалось распарсить ответ.");
            }
            else if (response.status == "success" && response.data != null){
              Console.WriteLine(response.data);
            }
            else if (response.status == "error"){
              Console.WriteLine("Сервер вернул ошибку: " + response.error);
            }
            else{
              Console.WriteLine("Неизвестный ответ от сервера.");
            }
          }
          catch (Exception ex){
            Console.WriteLine("Ошибка: " + ex.Message);
          }
          break;
        case "/chat-add":
          Console.Write("Write chat name: ");
          string ChatName2 = Console.ReadLine();
          Console.Write("Write chat name: ");
          string username = Console.ReadLine();
          ChatInfoType2 chatInfoType2 = new ChatInfoType2{
            SessionToken = token,
            ChatName = ChatName2,
            Username = username
          };
          try{
            string jsonRequest = JsonSerializer.Serialize(chatInfoType2);
            string jsonResponse = await PostRequestAsync($"https://{Program.ip}:5171/Chat/add-user-in-chat", jsonRequest);
            var options = new JsonSerializerOptions{
              PropertyNameCaseInsensitive = true
            };
            ChatResponseData response = JsonSerializer.Deserialize<ChatResponseData>(jsonResponse, options);
            if (response == null){
              Console.WriteLine("Ошибка: Не удалось распарсить ответ.");
            }
            else if (response.status == "success" && response.data != null){
              Console.WriteLine(response.data);
            }
            else if (response.status == "error"){
              Console.WriteLine("Сервер вернул ошибку: " + response.error);
            }
            else{
              Console.WriteLine("Неизвестный ответ от сервера.");
            }
          }
          catch (Exception ex){
            Console.WriteLine("Ошибка: " + ex.Message);
          }
          break;
        case "/all-chat":
          try{
            string jsonRequest = JsonSerializer.Serialize(userInfo);
            string jsonResponse = await PostRequestAsync($"https://{Program.ip}:5171/Chat/all-users-chats", jsonRequest);
            var options = new JsonSerializerOptions{
              PropertyNameCaseInsensitive = true
            };
            ChatsResponseData response = JsonSerializer.Deserialize<ChatsResponseData>(jsonResponse, options);
            if (response == null){
              Console.WriteLine("Ошибка: Не удалось распарсить ответ.");
            }
            else if (response.status == "success" && response.data?.chats != null){
              foreach(var chat in response.data.chats){
                Console.WriteLine("Id: "+ chat.ChatID + " Chat Name: " + chat.ChatName);
              }
            }
            else if (response.status == "error"){
              Console.WriteLine("Сервер вернул ошибку: " + response.error);
            }
            else{
              Console.WriteLine("Неизвестный ответ от сервера.");
            }
          }
          catch (Exception ex){
            Console.WriteLine("Ошибка: " + ex.Message);
          }
          break;
        case "/help": Console.WriteLine("All commands: \n" + 
                                        "/chat - enter in chat\n" + 
                                        "/new-chat - create new chat\n" + 
                                        "/chat-add - add user in chat\n" + 
                                        "/all-chat - all chats"); break;
        default: command = ""; break;
      }
    }
  }
  public void WriteMessage(){

  }
  public static async Task GetLastMessagesInChat(string token, string chatID){
    MessageType2 messageType2 = new MessageType2{
      SessionToken = token,
      ChatID = chatID
    };
    try{
      string jsonRequest = JsonSerializer.Serialize(messageType2);
      string jsonResponse = await PostRequestAsync($"https://{Program.ip}:5171/Message/get-chat-messages", jsonRequest);
      var options = new JsonSerializerOptions{
        PropertyNameCaseInsensitive = true
      };
      ResponseMessageData response = JsonSerializer.Deserialize<ResponseMessageData>(jsonResponse, options);
      if (response == null){
        Console.WriteLine("Ошибка: Не удалось распарсить ответ.");
      }
      else if (response.status == "success" && response.data?.messages != null){
        foreach (var message in response.data.messages){
          Console.WriteLine($"[{message.TimeStamp}] {message.Sender.Username}: {message.Content}");
        }
      }
      else if (response.status == "error"){
        Console.WriteLine("Сервер вернул ошибку: " + response.error);
      }
      else{
        Console.WriteLine("Неизвестный ответ от сервера.");
      }
    }
    catch (Exception ex){
      Console.WriteLine("Ошибка: " + ex.Message);
    }
  }

  public static async Task GetNewMessages(string token, string chatID){
    while(true){
      MessageType3 messageType3 = new MessageType3{
        SessionToken = token,
        ChatID = chatID,
        Time = DateTime.Now
      };
      try{
        string jsonRequest = JsonSerializer.Serialize(messageType3);
        string jsonResponse = await PostRequestAsync($"https://{Program.ip}:5171/Message/get-new-chat-messages", jsonRequest);
        var options = new JsonSerializerOptions{
          PropertyNameCaseInsensitive = true
        };
        ResponseMessageData response = JsonSerializer.Deserialize<ResponseMessageData>(jsonResponse, options);
        if (response == null){
          Console.WriteLine("Ошибка: Не удалось распарсить ответ.");
        }
        else if (response.status == "success" && response.data?.messages != null){
          foreach (var message in response.data.messages){
            Console.WriteLine($"[{message.TimeStamp}] {message.Sender.Username}: {message.Content}");
          }
        }
        else if (response.status == "error"){
          Console.WriteLine("Сервер вернул ошибку: " + response.error);
        }
        else{
          Console.WriteLine("Неизвестный ответ от сервера.");
        }
      }
      catch (Exception ex){
        Console.WriteLine("Ошибка: " + ex.Message);
      }
      await Task.Delay(500);
    }
  }

  private static async Task<string> PostRequestAsync(string url, string json){
    using var content = new StringContent(json, Encoding.UTF8, "application/json");
    using var response = await client.PostAsync(url, content);
    return await response.Content.ReadAsStringAsync();
  }

  public class MessageInfo{
    public string SessionToken {get; set;}
    public string Content {get; set;}
    public string ChatID {get; set;}
  }
  public class MessageType2{
    public string SessionToken {get; set;}
    public string ChatID {get;set;}
  }
  public class MessageType3{
    public string SessionToken {get; set;}
    public string ChatID {get;set;}
    public DateTime Time { get;set; }
  }

  public class ResponseMessageData{
    public string status { get; set; }
    public ResponseData data { get; set; }
    public string error {get; set;}
  }

  public class ResponseData{
    public List<MessageDto> messages { get; set; }
  }

  public class SenderDto{
    public int UserID { get; set; }
    public string Username { get; set; }
    public string UserProfilePicturePath { get; set; }
  }

  public class MessageDto{
    public int MessageID { get; set; }
    public int UserID { get; set; }
    public string Content { get; set; }
    public DateTime TimeStamp { get; set; }
    public int ChatID { get; set; }
    public bool IsSeen { get; set; }
    public bool IsFile { get; set; }
    public SenderDto Sender { get; set; }
  }

  public class ChatInfoType1{
    public string SessionToken {get; set;}
    public string ChatName {get; set;}
  }

  public class ChatInfoType2{
    public string SessionToken {get; set;}
    public string ChatName {get; set;}
    public string Username {get; set;}
  }
  public class ChatResponseData{
    public string status { get; set; }
    public string data { get; set; }
    public string error {get; set;}
  }
  public class ChatsResponseData{
    public string status { get; set; }
    public ResponseChatData data { get; set; }
    public string error {get; set;}
  }
  public class ResponseChatData{
    public List<ChatDto> chats { get; set; }
  }
  public class ChatDto{
    public int ChatID {get; set;}
    public string ChatName {get; set;}
  }
  public class UserInfo{
    public string SessionToken {get;set;}
  }
}
