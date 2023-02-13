<h1>Description</h1>
<hr/>

- The project is designed as a service to host Telegram bots that can solve a variety of small to medium tasks for everyday life.
- The project provides the necessary infrastructure to create a new Telegram bot easily and fast in a plugin-style way.
- Currently consists of the next bots:
  - LanguageTeacherBot allows users to add/delete English to Ukrainian word translations and learn them by using the repetitive algorithm. It uses Azure Translation services along with several additions to get audio and examples for words.
  - NewYearMoviesBot is created to send Christmas and New Year movies like an advent calendar. A user receives movies to watch every day from 4 December till 7 January. 
  - InstagramHelperBot can be used to get the most popular hashtags by keywords as well as captions for them. 
  - IndoTaxHelperBot is a tiny bot to add Indonesian taxes to the price. It's very useful in cafes because all prices usually don't include taxes by default.  

<h1>Core Technologies</h1>
<hr/>

- ASP.NET Core (.NET 6);
- EntityFramework with MS SQL;
- MediatR.
  
![telegrambots drawio](https://user-images.githubusercontent.com/23040546/203381341-67a7f80f-0de6-42ff-8203-b2940ee2190a.png)
