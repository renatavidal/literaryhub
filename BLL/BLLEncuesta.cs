using System;
using System.Collections.Generic;
using System.Data;
using BE;
using MPP;

namespace BLL
{
    public class BLLEncuesta
    {
        private readonly MPPEncuesta _mpp = new MPPEncuesta();

        public int Create(BESurvey s, int createdBy)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (string.IsNullOrWhiteSpace(s.Title)) throw new ArgumentException("Título requerido.");
            if (s.Questions.Count == 0 || s.Questions.Count > 5)
                throw new ArgumentException("Debe haber entre 1 y 5 preguntas.");

            foreach (var q in s.Questions)
            {
                if (string.IsNullOrWhiteSpace(q.Text)) throw new ArgumentException("Texto requerido.");
                if (q.QType != SurveyQType.YesNo && q.QType != SurveyQType.Rating)
                    throw new ArgumentException("Tipo inválido.");
            }
            return _mpp.Create(s, createdBy);
        }


        public void Delete(int surveyId) => _mpp.Delete(surveyId);

        public DataTable List(bool? onlyActive = null) => _mpp.List(onlyActive);

        public BESurvey NextForUser(int userId) => userId > 0 ? _mpp.NextForUser(userId) : null;

        public void Submit(int surveyId, int userId, byte?[] answers)
        {
            if (surveyId <= 0 || userId <= 0) throw new ArgumentException("Datos inválidos.");
            if (answers == null) answers = Array.Empty<byte?>();
            // Validaciones básicas (sí/no -> 0/1, rating -> 1..5) pueden ser adicionales si quisieras cargar el QType.
            _mpp.SubmitAnswers(surveyId, userId, answers);
        }

        public void Skip(int surveyId, int userId)
        {
            if (surveyId <= 0 || userId <= 0) throw new ArgumentException("Datos inválidos.");
            _mpp.OptOut(surveyId, userId);
        }

        public List<BESurveyQuestionStats> Stats(int surveyId) => _mpp.GetStats(surveyId);

        public BESurvey Get(int surveyId) => _mpp.Get(surveyId);
    }
}
