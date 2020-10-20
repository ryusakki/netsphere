using System;
using System.Net;
using System.Collections.Generic;

public class Peer
{
    // EndPoint do peer usado para registrar-se no servidor.
    // Cada peer fornece um conteúdo através do endpoint registrado (talvez uma tupla de ip:port seja melhor que um IPEndPoint)
    public IPEndPoint Ip { get; set; }

    // Data do último ping enviado pelo heartbeat do peer
    public DateTime Timestamp { get; set; }

    // Lista contendo o hash de cada arquivo disponibilizado pelo peer
    public List<string> AvailableContent { get; set; }
}