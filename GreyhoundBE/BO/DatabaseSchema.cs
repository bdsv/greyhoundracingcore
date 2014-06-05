using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreyhoundBE.BO
{
    class DatabaseSchema
    {
        //Query to get this session's last inserted id
        public const string SELECT_USERS_NEXT_ID = "SELECT USERS_SEQ.nextval from dual";
        public const string SELECT_USERS_LAST_INSERTED_ID = "SELECT USERS_SEQ.currval from dual";

        public const char USER_NOT_REGISTERED = '0';
        public const char USER_REGISTERED = '1';

        //table Users
        public const string USERS_TABLE = "Users";
        public const string USERS_USER_ID_FIELD = "user_id";
        public const string USERS_PASSWORD_FIELD = "password";
        public const string USERS_ROLE_ID_FIELD = "roles_role_id";
        public const string USERS_NAME_FIELD = "name";
        public const string USERS_EMAIL_FIELD = "email";
        public const string USERS_MOBILE_FIELD = "mobile";
        public const string USERS_REGISTERED_FIELD = "registered";
        public const string USERS_ADDRESS_FIELD = "address";
        public const string USERS_PAYPAL_ID_FIELD = "paypal_id";
        public const string USERS_BETFAIR_ID_FIELD = "betfair_id";

        //table Greyhounds
        public const string GREYHOUNDS_TABLE = "Greyhounds";
        public const string GREYHOUNDS_GREYHOUND_ID_FIELD = "grey_id";
        public const string GREYHOUNDS_NAME_FIELD = "name";
        public const string GREYHOUNDS_TRAINER_FIELD = "trainer";
        public const string GREYHOUNDS_BIRTH_DATE_FIELD = "birth_date";
        public const string GREYHOUNDS_SCORE_FIELD = "score";

        //table Stadiums
        public const string STADIUMS_TABLE = "Stadiums";
        public const string STADIUMS_STADIUM_ID_FIELD = "std_id";
        public const string STADIUMS_NAME_FIELD = "name";
        public const string STADIUMS_NUM_TRACKS_FIELD = "num_tracks";
        public const string STADIUMS_CITY_FIELD = "city";
        public const string STADIUMS_COUNTRY_FIELD = "country";

        //table Races
        public const string RACES_TABLE = "Races";
        public const string RACES_ID_FIELD = "race_id";
        public const string RACES_STADIUM_ID_FIELD = "stadiums_std_id";
        public const string RACES_DATE_TIME_FIELD = "date_time";
        public const string RACES_RACE_NUMBER_FIELD = "race_number";
        public const string RACES_TRACK_LENGTH_FIELD = "track_length";
        public const string RACES_GRADE_FIELD = "grade";
        public const string RACES_RACE_TYPE_FIELD = "race_type";

        //table Races_Details
        public const string RACES_DETAILS_TABLE = "Races_Details";
        public const string RACES_DETAILS_RACES_RACE_ID_FIELD = "races_race_id";
        public const string RACES_DETAILS_GREYHOUNDS_GREY_ID_FIELD = "greyhounds_grey_id";
        public const string RACES_DETAILS_TRACK_NUMBER_FIELD = "track_number";
        public const string RACES_DETAILS_RACE_COMPLETED_FIELD = "race_completed";
        public const string RACES_DETAILS_POSITION_FIELD = "position";
        public const string RACES_DETAILS_TIME_FIELD = "time";

        //table Odds
        public const string ODDS_TABLE = "Odds";
        public const string ODDS_RACES_DETAILS_RACE_ID_FIELD = "races_details_race_id";
        public const string ODDS_RACES_DETAILS_GREY_ID_FIELD = "races_details_grey_id";
        public const string ODDS_VALUE_FIELD = "odd";

        //table Predictions
        public const string PREDICTIONS_TABLE = "Predictions";
        public const string PREDICTIONS_RACES_DETAILS_RACE_ID_FIELD = "races_details_race_id";
        public const string PREDICTIONS_RACES_DETAILS_GREY_ID_FIELD = "races_details_grey_id";
        public const string PREDICTIONS_SIMPLE_VALUE_FIELD = "prediction_simple";
        public const string PREDICTIONS_ADVANCED_VALUE_FIELD = "prediction_advanced";

        //table Bets
        public const string BETS_TABLE = "Bets";
        public const string BETS_BET_ID = "BET_ID";
        public const string BETS_USER_ID = "USERS_USER_ID";
        public const string BETS_BET_TYPE_ID = "BET_TYPES_BET_TYPE_ID";
        public const string BETS_RACE_ID = "RACES_DETAILS_RACE_ID";
        public const string BETS_GREY_ID = "RACES_DETAILS_GREY_ID";
        public const string BETS_DATE_TIME = "DATE_TIME";
        public const string BETS_BET_VALUE = "BET_VALUE";
        public const string BETS_BET_RESULT = "BET_RESULT";
        public const string BETS_RETURN_VALUE = "RETURN_VALUE";

        //view V_RACES_DETAILS
        public const string RACES_DETAILS_VIEW = "V_RACES_DETAILS";
        public const string V_RACES_DETAILS_RACES_ID_FIELD = "race_id";
        public const string V_RACES_DETAILS_RACES_DATE_TIME_FIELD = "date_time";
    }
}
