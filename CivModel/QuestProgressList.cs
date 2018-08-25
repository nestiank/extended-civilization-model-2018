using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// Represents a list of <see cref="QuestProgress"/>.
    /// This collection is also dictionary with <see cref="QuestProgress.Id"/> as a key.
    /// </summary>
    /// <seealso cref="Quest.Progresses"/>
    /// <seealso cref="QuestProgress"/>
    /// <seealso cref="IReadOnlyList{QuestProgress}"/>
    /// <seealso cref="IReadOnlyDictionary{String, QuestProgress}"/>
    public class QuestProgressList : IReadOnlyList<QuestProgress>
    {
        private QuestProgress[] _progresses;
        private Dictionary<string, QuestProgress> _dict = new Dictionary<string, QuestProgress>();

        internal QuestProgressList(Quest quest, IReadOnlyList<QuestProgressPrototype> protoList)
        {
            _progresses = new QuestProgress[protoList.Count];
            for (int i = 0; i < protoList.Count; ++i)
            {
                var x = new QuestProgress(quest, protoList[i]);
                _progresses[i] = x;
                _dict[x.Id] = x;
            }
        }

        /// <summary>
        /// 읽기 전용 목록에서 지정된 인덱스의 요소를 가져옵니다.
        /// </summary>
        /// <param name="index">가져올 요소의 0부터 시작하는 인덱스입니다.</param>
        /// <returns>읽기 전용 목록에서 지정된 인덱스의 요소입니다.</returns>
        public QuestProgress this[int index] => _progresses[index];

        /// <summary>
        /// 읽기 전용 사전에 지정된 된 키를 가진 요소를 가져옵니다.
        /// </summary>
        /// <param name="key">찾을 키입니다.</param>
        /// <returns>읽기 전용 사전에 지정된 된 키가 있는 요소입니다.</returns>
        public QuestProgress this[string key] => _dict[key];

        /// <summary>
        /// This collection as dictionary.
        /// </summary>
        public IReadOnlyDictionary<string, QuestProgress> AsDictionary => _dict;

        /// <summary>
        /// 컬렉션의 요소 수를 가져옵니다.
        /// </summary>
        public int Count => _progresses.Length;

        /// <summary>
        /// 읽기 전용 사전의 키를 포함하는 열거 가능한 컬렉션을 가져옵니다.
        /// </summary>
        public IEnumerable<string> Keys => _dict.Keys;

        /// <summary>
        /// 읽기 전용 사전의 값을 포함하는 열거 가능한 컬렉션을 가져옵니다.
        /// </summary>
        public IEnumerable<QuestProgress> Values => _dict.Values;

        /// <summary>
        /// 읽기 전용 사전의 지정된 된 키가 있는 요소가 포함 되어 있는지 여부를 결정 합니다.
        /// </summary>
        /// <param name="key">찾을 키입니다.</param>
        /// <returns>
        /// <see langword="true" /> 읽기 전용 사전에 지정된 된 키를 가진 요소가 포함 되어 있는 경우 그렇지 않으면 <see langword="false" />합니다.
        /// </returns>
        public bool ContainsKey(string key)
            => _dict.ContainsKey(key);

        /// <summary>
        /// 컬렉션을 반복하는 열거자를 반환합니다.
        /// </summary>
        /// <returns>
        /// 컬렉션을 반복하는 데 사용할 수 있는 열거자입니다.
        /// </returns>
        public IEnumerator<QuestProgress> GetEnumerator()
            => ((IEnumerable<QuestProgress>)_progresses).GetEnumerator();

        /// <summary>
        /// 지정된 된 키와 연결 된 값을 가져옵니다.
        /// </summary>
        /// <param name="key">찾을 키입니다.</param>
        /// <param name="value">이 메서드가 반환될 때 지정된 키가 있으면 해당 키와 연결된 값이고, 그렇지 않으면 <paramref name="value" /> 매개 변수의 형식에 대한 기본값입니다.
        /// 이 매개 변수는 초기화되지 않은 상태로 전달됩니다.</param>
        /// <returns>
        /// <see langword="true" /> 구현 하는 개체는 <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2" /> 가 지정 하는 요소를 포함 하는 인터페이스 키이 고, 그렇지 않으면 <see langword="false" />합니다.
        /// </returns>
        public bool TryGetValue(string key, out QuestProgress value)
            => _dict.TryGetValue(key, out value);

        /// <summary>
        /// 컬렉션을 반복하는 열거자를 반환합니다.
        /// </summary>
        /// <returns>
        /// 컬렉션을 반복하는 데 사용할 수 있는 <see cref="T:System.Collections.IEnumerator" /> 개체입니다.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
