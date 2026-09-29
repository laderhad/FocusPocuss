import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import {
  completeFocusSession,
  getFocusSession,
  getFocusSessionHistory,
  recordFocusSessionReflection,
  reportDistraction,
  startFocusSession,
  type DistractionReason,
  type FocusSessionReflection,
  type FocusSessionDto,
} from './focusApi';

const focusSessionKeys = {
  history: ['focus-sessions', 'history'] as const,
  detail: (sessionId: number) => ['focus-sessions', sessionId] as const,
};

export function useFocusSessionHistory() {
  return useQuery({
    queryKey: focusSessionKeys.history,
    queryFn: getFocusSessionHistory,
  });
}

export function useFocusSession(sessionId: number) {
  return useQuery({
    queryKey: focusSessionKeys.detail(sessionId),
    queryFn: () => getFocusSession(sessionId),
    enabled: Number.isInteger(sessionId) && sessionId > 0,
  });
}

export function useStartFocusSession() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: startFocusSession,
    onSuccess: (session) => {
      if (session.id !== undefined) {
        queryClient.setQueryData<FocusSessionDto>(
          focusSessionKeys.detail(session.id),
          session,
        );
      }
      void queryClient.invalidateQueries({ queryKey: focusSessionKeys.history });
    },
  });
}

export function useCompleteFocusSession(sessionId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: () => completeFocusSession(sessionId),
    onSuccess: (session) => {
      queryClient.setQueryData<FocusSessionDto>(
        focusSessionKeys.detail(sessionId),
        session,
      );
      void queryClient.invalidateQueries({ queryKey: focusSessionKeys.history });
    },
  });
}

export function useReportDistraction(sessionId: number) {
  return useMutation({
    mutationFn: (reason: DistractionReason) => reportDistraction(sessionId, reason),
  });
}

export function useRecordFocusSessionReflection(sessionId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (reflection: FocusSessionReflection) =>
      recordFocusSessionReflection(sessionId, reflection),
    onSuccess: (session) => {
      queryClient.setQueryData<FocusSessionDto>(
        focusSessionKeys.detail(sessionId),
        session,
      );
      void queryClient.invalidateQueries({ queryKey: focusSessionKeys.history });
    },
  });
}
