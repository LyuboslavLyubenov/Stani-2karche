//
//  MessageEventArgs.cs
//
//  Author:
//       Любослав Любенов <dead4y@mail.bg>
//
//  Copyright (c) 2016 Dead4y
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using UnityEngine;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using CielaSpike;
using System.Net;
using System;
using System.Text;
using System.Linq;
using System.Threading;
using System.Security.Cryptography;

public class MessageEventArgs : IpEventArgs
{
    public MessageEventArgs(string ip, string message)
        : base(ip)
    {
        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentNullException("message");
        }

        this.Message = message;
    }

    public string Message
    {
        get;
        private set;
    }
}
