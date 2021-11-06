﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework.API
{
    public enum ApiResultStatusCode
    {
        None = -1,

        // 1xx Authentication error
        WrongUsernameOrPassword = 100,

        // 2xx Authorization error
        InvalidToken = 200,
        TokenExpired = 201,
        TokenStampHasChanged = 202,
        AccessDenied = 203,
        TokenRequired = 204,
        AccountRestricted = 205,

        // 3xx Generic error
        UserNameExist = 300,
    }
}
