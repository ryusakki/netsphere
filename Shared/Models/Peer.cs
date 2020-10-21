using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

public class Peer
{
    // EndPoint do peer usado para registrar-se no servidor.
    // Que na realidade é apenas uma string no formato ip:port
    [Required]
    public string IPEndPoint { get; set; }

    // Data do último ping enviado pelo heartbeat do peer
    [Required]
    public DateTime Timestamp { get; set; }

    // Lista contendo o hash de cada arquivo disponibilizado pelo peer
    [Required]
    public List<string> AvailableContent { get; set; }

    public override bool Equals([DisallowNull] object o)
    {
        return (o as Peer).IPEndPoint == IPEndPoint;
    }

    public override int GetHashCode()
    {
        return IPEndPoint.GetHashCode();
    }
}