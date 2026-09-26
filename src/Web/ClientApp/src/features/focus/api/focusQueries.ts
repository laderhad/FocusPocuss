import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import {
  completeFocusSession,
  getFocusSession,
  startFocusSession,
  type FocusSessionDto,
} from './focusApi';

const focusSessionKeys = {
  detail: (sessionId: number) => ['focus-sessions', sessionId] as const,
};

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
    },
  });
}
