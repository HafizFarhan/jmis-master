using JMICSAPP.Data;
using JMICSAPP.Models;
using MTC.JMICS.Models.DB;
using MTC.JMICS.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JMICSAPP.Hubs
{
    public interface IPushHub
    {
        Task PushAISTrack(AISTrackRequest track);
        Task PushAISTrackUpdate(AISTrackRequest track);
        Task PushCOIs(COI COI);
        Task PushLR(LostContactReport lostReport);
        Task PushDR(DropInfoSharingReport dropReport);
        Task PushNews(NewsViewModel newsViewModel);
        Task PushSR(SubsequentReport subsequentReport);
        Task PushPR(PreliminaryReport preliminaryReport);
        Task DeletePR(string prNumber);
        Task DeleteSR(string srNumber);
        Task DeleteCOI(string coiNumber);
        Task DeleteAR(string arNumber);
        Task PushAR(AmplifyingReport amplifyingReport);
        Task PushNewsFeed(NewsFeed newsFeed);
        Task PushDelete(int id);
        Task PushDrawing(DrawingCoordinate dCoord);
        Task PushNotification(Notifications notifications);
        Task PushAAR(AfterActionReport afterActionReport);
        Task PushNatureOfThreat(NatureOfThreat threatLevel);
        Task PushCOIType(COIType COIType);
        Task PushStakeholder(Subscriber subscriber);
        Task PushUser(AppUser appUser);
        Task PushUserType(UserType userType);
        Task PushNewsFeedType(NewsFeedType newsFeedType);        
        Task PushCOIStatus(COIStatus COIStatus);
        Task PushInfoConLevel(InfoConfidenceLevel infoConLevel);
        Task PushAAASSOS(AAASSOSRequest SOS);
        Task PushAAASIncident(AAASIncidentRequest incident);
        Task PushTemplate(Template template);
        Task PushTemplateType(TemplateType templateType);
        Task PushNotes(Notes notes);
    }
}
