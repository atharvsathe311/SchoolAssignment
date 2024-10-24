﻿namespace SchoolAPI.DTO
{
    public class StudentGetDTO
    {
        public int StudentId { get; set; }  
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
    }
}
